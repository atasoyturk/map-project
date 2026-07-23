using System.Collections.Concurrent;
using BackendApi.Data;
using BackendApi.DTOs.Transit;
using BackendApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.LinearReferencing;

namespace BackendApi.Services.Transit;

public sealed class RouteSimulationService : IRouteSimulationService
{
    private const double DurationScaleFactor = 360;
    private const double MinSimulationSeconds = 3;
    private const int    TickIntervalMs       = 250;

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

    public async Task<(bool Success, string? Error)> StartAsync(int routeId, int startedByUserId)
    {
        if (_running.ContainsKey(routeId))
            return (false, "Bu güzergah için zaten çalışan bir simülasyon var.");

        using var scope   = _scopeFactory.CreateScope();
        var       context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var route = await context.TransitRoutes
            .FirstOrDefaultAsync(r => r.Id == routeId && !r.IsDeleted);

        if (route is null)
            return (false, "Güzergah bulunamadı.");

        if (route.RouteGeometry is not LineString lineString || route.DurationSeconds is null)
            return (false, "Bu güzergah için önce bir rota oluşturulmalıdır.");

        var scaledDuration = Math.Max(route.DurationSeconds.Value / DurationScaleFactor, MinSimulationSeconds);

        var state = new RouteSimulationState
        {
            RouteId         = routeId,
            StartedByUserId = startedByUserId,
            StartedAtUtc    = DateTime.UtcNow,
            DurationSeconds = scaledDuration,
            RouteGeometry   = lineString,
        };

        var cts     = new CancellationTokenSource();
        var running = new RunningSimulation(state, cts);

        if (!_running.TryAdd(routeId, running))
            return (false, "Bu güzergah için zaten çalışan bir simülasyon var.");

        _ = RunSimulationLoopAsync(running, cts.Token);

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> StopAsync(int routeId)
    {
        if (!_running.TryRemove(routeId, out var running))
            return (false, "Çalışan bir simülasyon bulunamadı.");

        running.Cts.Cancel();
        await BroadcastAsync(routeId, "SimulationStopped", new { routeId });

        return (true, null);
    }

    public VehiclePositionDto? GetStatus(int routeId) =>
        _running.TryGetValue(routeId, out var running) ? ComputePosition(running.State) : null;

    private async Task RunSimulationLoopAsync(RunningSimulation running, CancellationToken token)
    {
        var state = running.State;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var dto = ComputePosition(state);
                await BroadcastAsync(state.RouteId, "VehiclePositionUpdated", dto);

                if (dto.Completed) break;

                await Task.Delay(TickIntervalMs, token);
            }
        }
        catch (TaskCanceledException)
        {
            // Manuel durdurma (StopAsync) — normal akış, ekstra işlem gerekmez.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route simulation loop failed for route {RouteId}", state.RouteId);
        }
        finally
        {
            _running.TryRemove(state.RouteId, out _);
        }
    }

    private static VehiclePositionDto ComputePosition(RouteSimulationState state)
    {
        var indexedLine = new LengthIndexedLine(state.RouteGeometry);
        var totalLength = state.RouteGeometry.Length;

        var elapsedSeconds = (DateTime.UtcNow - state.StartedAtUtc).TotalSeconds;
        var fraction        = Math.Clamp(elapsedSeconds / state.DurationSeconds, 0, 1);
        var position         = indexedLine.ExtractPoint(fraction * totalLength);

        return new VehiclePositionDto(
            state.RouteId, position.Y, position.X, Math.Round(fraction * 100, 1), fraction >= 1);
    }

    private Task BroadcastAsync<T>(int routeId, string method, T payload) =>
        _hubContext.Clients.Group(RouteSimulationHub.GroupName(routeId)).SendAsync(method, payload);
}