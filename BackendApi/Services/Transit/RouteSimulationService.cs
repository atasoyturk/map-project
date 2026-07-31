using System.Collections.Concurrent;
using BackendApi.Data;
using BackendApi.DTOs.Transit;
using BackendApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.LinearReferencing;
using ShipmentRecordEntity = BackendApi.Entities.Transit.ShipmentRecord;
using BackendApi.Entities.Company;

namespace BackendApi.Services.Transit;

public sealed class RouteSimulationService : IRouteSimulationService
{
    // 1 saatlik gerçek seyahat süresi ~10 sn simülasyona karşılık geliyo
    private const double DurationScaleFactor  = 360;
    private const double MinSimulationSeconds = 15;
    private const double MaxSimulationSeconds = 90;
    private const int    TickIntervalMs       = 250;
    private const double MaxBusSpeedKmh = 50;

    private readonly IServiceScopeFactory                _scopeFactory;
    private readonly IHubContext<RouteSimulationHub>     _hubContext;
    private readonly ILogger<RouteSimulationService>     _logger;
    private readonly ConcurrentDictionary<int, RunningSimulation> _running = new();

    public RouteSimulationService(
        IServiceScopeFactory            scopeFactory,
        IHubContext<RouteSimulationHub> hubContext,
        ILogger<RouteSimulationService> logger)
    {
        _scopeFactory = scopeFactory;
        _hubContext   = hubContext;
        _logger       = logger;
    }

    private sealed record RunningSimulation(RouteSimulationState State, CancellationTokenSource Cts);

    public async Task<SimulationActionResponseDto> StartAsync(int routeId, int[] vehicleIds, int startedByUserId)
    {
        using var scope   = _scopeFactory.CreateScope();
        var       context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var route = await context.TransitRoutes.FirstOrDefaultAsync(r => r.Id == routeId && !r.IsDeleted);

        if (route?.RouteGeometry is not LineString lineString || route.DurationSeconds is null)
        {
            var routeError = "Bu güzergah için önce bir rota oluşturulmalıdır.";
            return new SimulationActionResponseDto(
                vehicleIds.Select(id => new VehicleActionResultDto(id, "", false, routeError)).ToList());
        }

        var eligibleVehicleIds = await context.CompanyRoutes
            .Where(cr => cr.TransitRouteId == routeId)
            .Join(context.Vehicles, cr => cr.CompanyId, v => v.CompanyId, (cr, v) => v.Id)
            .ToListAsync();

        var results = new List<VehicleActionResultDto>();

        foreach (var vehicleId in vehicleIds)
        {
            var vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId && !v.IsDeleted);
            if (vehicle is null)
            {
                results.Add(new VehicleActionResultDto(vehicleId, "", false, "Araç bulunamadı."));
                continue;
            }

            if (!eligibleVehicleIds.Contains(vehicleId))
            {
                results.Add(new VehicleActionResultDto(vehicleId, vehicle.PlateNumber, false,
                    "Bu aracın şirketi, bu güzergaha atanmamış."));
                continue;
            }

            if (_running.ContainsKey(vehicleId))
            {
                results.Add(new VehicleActionResultDto(vehicleId, vehicle.PlateNumber, false,
                    "Bu araç zaten bir sevkiyatta."));
                continue;
            }

            IReadOnlyList<SpeedSchedulePoint>? speedSchedule = null;
            double totalRealSeconds = route.DurationSeconds.Value;

            if (vehicle.VehicleType == VehicleType.Otobus)
            {
                var stops = await context.TransitStops
                    .Where(s => s.TransitRouteId == routeId && !s.IsDeleted)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();

                if (stops.Count >= 2)
                {
                    var rawSchedule = BuildSpeedSchedule(lineString, stops.Select(s => s.Geometry.Coordinate));
                    totalRealSeconds = rawSchedule[^1].ElapsedSeconds;

                    var scaledTotal = Math.Clamp(totalRealSeconds / DurationScaleFactor, MinSimulationSeconds, MaxSimulationSeconds);
                    var scaleRatio  = totalRealSeconds > 0 ? scaledTotal / totalRealSeconds : 1;

                    speedSchedule = rawSchedule
                        .Select(p => p with { ElapsedSeconds = p.ElapsedSeconds * scaleRatio })
                        .ToList();
                }
            }

            var scaledDuration = Math.Clamp(totalRealSeconds / DurationScaleFactor, MinSimulationSeconds, MaxSimulationSeconds);

            var state = new RouteSimulationState
            {
                RouteId         = routeId,
                VehicleId       = vehicleId,
                PlateNumber     = vehicle.PlateNumber,
                StartedByUserId = startedByUserId,
                StartedAtUtc    = DateTime.UtcNow,
                DurationSeconds = scaledDuration,
                RouteGeometry   = lineString,
                SpeedSchedule   = speedSchedule,
            };

            var cts     = new CancellationTokenSource();
            var running = new RunningSimulation(state, cts);

            if (!_running.TryAdd(vehicleId, running))
            {
                results.Add(new VehicleActionResultDto(vehicleId, vehicle.PlateNumber, false,
                    "Bu araç zaten bir sevkiyatta."));
                continue;
            }

            _ = RunSimulationLoopAsync(running, cts.Token);
            results.Add(new VehicleActionResultDto(vehicleId, vehicle.PlateNumber, true, null));
        }

        return new SimulationActionResponseDto(results);
    }

    public async Task<SimulationActionResponseDto> StopAsync(int routeId, int[] vehicleIds)
    {
        var results = new List<VehicleActionResultDto>();

        foreach (var vehicleId in vehicleIds)
        {
            if (!_running.TryGetValue(vehicleId, out var running) || running.State.RouteId != routeId)
            {
                results.Add(new VehicleActionResultDto(vehicleId, "", false, "Çalışan bir sevkiyat bulunamadı."));
                continue;
            }

            _running.TryRemove(vehicleId, out _);
            running.Cts.Cancel();

            await BroadcastAsync(routeId, "SimulationStopped", new { routeId, vehicleId });
            results.Add(new VehicleActionResultDto(vehicleId, running.State.PlateNumber, true, null));
        }

        return new SimulationActionResponseDto(results);
    }

    public IEnumerable<VehiclePositionDto> GetStatus(int routeId) =>
        _running.Values
            .Where(r => r.State.RouteId == routeId)
            .Select(r => ComputePosition(r.State));
    
    private static IReadOnlyList<SpeedSchedulePoint> BuildSpeedSchedule(
        LineString routeGeometry, IEnumerable<Coordinate> stopCoordinates)
    {
        var indexedLine   = new LengthIndexedLine(routeGeometry);
        var maxSpeedMps   = (MaxBusSpeedKmh * 1000) / 3600.0;

        var stopDistances = stopCoordinates
            .Select(c => indexedLine.Project(c))
            .OrderBy(d => d)
            .ToList();

        var schedule = new List<SpeedSchedulePoint> { new(stopDistances[0], 0) };

        for (var i = 1; i < stopDistances.Count; i++)
        {
            var segmentMeters  = stopDistances[i] - stopDistances[i - 1];
            var segmentSeconds = segmentMeters / maxSpeedMps;
            schedule.Add(new SpeedSchedulePoint(
                stopDistances[i], schedule[^1].ElapsedSeconds + segmentSeconds));
        }

        return schedule;
    }

    private async Task RunSimulationLoopAsync(RunningSimulation running, CancellationToken token)
    {
        var state = running.State;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var dto = ComputePosition(state);
                await BroadcastAsync(state.RouteId, "VehiclePositionUpdated", dto);

                if (dto.Completed)
                {
                    await RecordShipmentAsync(state);
                    break;
                }

                await Task.Delay(TickIntervalMs, token);
            }
        }
        catch (TaskCanceledException)
        {
            // Manuel durdurma (StopAsync) — normal akış, ekstra işlem gerekmez.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route simulation loop failed for vehicle {VehicleId}", state.VehicleId);
        }
        finally
        {
            _running.TryRemove(state.VehicleId, out _);
        }
    }

    private async Task RecordShipmentAsync(RouteSimulationState state)
    {
        try
        {
            using var scope   = _scopeFactory.CreateScope();
            var       context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.ShipmentRecords.Add(new ShipmentRecordEntity
            {
                TransitRouteId = state.RouteId,
                VehicleId      = state.VehicleId,
                StartedAtUtc   = state.StartedAtUtc,
                CompletedAtUtc = DateTime.UtcNow,
            });

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record completed shipment for vehicle {VehicleId}", state.VehicleId);
        }
    }

    private static VehiclePositionDto ComputePosition(RouteSimulationState state)
    {
        var indexedLine     = new LengthIndexedLine(state.RouteGeometry);
        var elapsedSeconds  = (DateTime.UtcNow - state.StartedAtUtc).TotalSeconds;

        double distanceMeters;
        double fraction;

        if (state.SpeedSchedule is { Count: > 1 } schedule)
        {
            
            var totalScaledSeconds = schedule[^1].ElapsedSeconds;
            var clampedElapsed     = Math.Clamp(elapsedSeconds, 0, totalScaledSeconds);

            distanceMeters = InterpolateDistance(schedule, clampedElapsed);
            fraction        = totalScaledSeconds > 0 ? clampedElapsed / totalScaledSeconds : 1;
        }
        else
        {
            var fractionOfDuration = Math.Clamp(elapsedSeconds / state.DurationSeconds, 0, 1);
            distanceMeters = fractionOfDuration * state.RouteGeometry.Length;
            fraction        = fractionOfDuration;
        }

        var position = indexedLine.ExtractPoint(distanceMeters);

        return new VehiclePositionDto(
            state.RouteId, state.VehicleId, state.PlateNumber,
            position.Y, position.X, Math.Round(fraction * 100, 1), fraction >= 1);
    }

    /// Verilen (ölçeksiz, gerçek) geçen süreye karşılık gelen mesafeyi, takvimdeki
    /// iki nokta arasında doğrusal enterpolasyonla bulur.
    private static double InterpolateDistance(IReadOnlyList<SpeedSchedulePoint> schedule, double elapsedSeconds)
    {
        for (var i = 1; i < schedule.Count; i++)
        {
            if (elapsedSeconds > schedule[i].ElapsedSeconds) continue;

            var prev = schedule[i - 1];
            var curr = schedule[i];
            var segmentDuration = curr.ElapsedSeconds - prev.ElapsedSeconds;

            if (segmentDuration <= 0) return curr.DistanceMeters;

            var t = (elapsedSeconds - prev.ElapsedSeconds) / segmentDuration;
            return prev.DistanceMeters + t * (curr.DistanceMeters - prev.DistanceMeters);
        }

        return schedule[^1].DistanceMeters;
    }

    private Task BroadcastAsync<T>(int routeId, string method, T payload) =>
        _hubContext.Clients.Group(RouteSimulationHub.GroupName(routeId)).SendAsync(method, payload);
}