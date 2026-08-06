using BackendApi.DTOs.Transit;
using NetTopologySuite.Geometries;

namespace BackendApi.Services.Transit;

public sealed class ThematicMapService : IThematicMapService
{
    private const int    RayCount        = 16;
    private const double MaxRadiusMeters = 12_000;
    private static readonly int[] MinuteThresholds = [5, 10, 15];

    private readonly IOsrmService _osrmService;
    private readonly ILogger<ThematicMapService> _logger;

    private static readonly GeometryFactory GeometryFactory = new(new PrecisionModel(), 4326);

    public ThematicMapService(IOsrmService osrmService, ILogger<ThematicMapService> logger)
    {
        _osrmService = osrmService;
        _logger      = logger;
    }

    public async Task<IsochroneResultDto> GetIsochroneAsync(double centerLon, double centerLat)
    {
        var center = new Coordinate(centerLon, centerLat);
        var rayTargets = BuildRayTargets(center);

        var rayResults = await Task.WhenAll(rayTargets.Select(target =>
            _osrmService.GetRouteWithCumulativeDurationsAsync(center, target)));

        var bands = new List<IsochroneBandDto>();

        foreach (var minutes in MinuteThresholds.OrderByDescending(m => m))
        {
            var targetSeconds = minutes * 60.0;
            var ringPoints = new List<Coordinate>();

            foreach (var (success, coordinates, cumulativeDurations, error) in rayResults)
            {
                if (!success || coordinates is null || cumulativeDurations is null)
                {
                    _logger.LogWarning("Isochrone ray skipped: {Error}", error);
                    continue;
                }

                var point = InterpolatePointAtDuration(coordinates, cumulativeDurations, targetSeconds);
                if (point is not null) ringPoints.Add(point);
            }

            if (ringPoints.Count < 3) continue; 

            ringPoints.Add(ringPoints[0]); 
            var polygon = GeometryFactory.CreatePolygon(ringPoints.ToArray());

            bands.Add(new IsochroneBandDto(minutes, polygon.AsText()));
        }

        return new IsochroneResultDto(bands);
    }

    
    private static Coordinate? InterpolatePointAtDuration(
        IReadOnlyList<Coordinate> coordinates, IReadOnlyList<double> cumulativeDurations, double targetSeconds)
    {
        if (coordinates.Count == 0) return null;

        if (targetSeconds >= cumulativeDurations[^1])
            return coordinates[^1];

        for (var i = 1; i < cumulativeDurations.Count; i++)
        {
            if (cumulativeDurations[i] < targetSeconds) continue;

            var segmentStart = cumulativeDurations[i - 1];
            var segmentEnd   = cumulativeDurations[i];
            var segmentSpan  = segmentEnd - segmentStart;

            if (segmentSpan <= 0) return coordinates[i];

            var t = (targetSeconds - segmentStart) / segmentSpan;
            var from = coordinates[i - 1];
            var to   = coordinates[i];

            return new Coordinate(
                from.X + t * (to.X - from.X),
                from.Y + t * (to.Y - from.Y));
        }

        return coordinates[^1];
    }

        private static List<Coordinate> BuildRayTargets(Coordinate center)
    {
        var targets = new List<Coordinate>();
        var latRadians = center.Y * Math.PI / 180;

        var metersPerDegreeLat = 111_320.0;
        var metersPerDegreeLon = 111_320.0 * Math.Cos(latRadians);

        for (var i = 0; i < RayCount; i++)
        {
            var angle = 2 * Math.PI * i / RayCount;
            var dx = MaxRadiusMeters * Math.Cos(angle);
            var dy = MaxRadiusMeters * Math.Sin(angle);

            targets.Add(new Coordinate(
                center.X + dx / metersPerDegreeLon,
                center.Y + dy / metersPerDegreeLat));
        }

        return targets;
    }
}