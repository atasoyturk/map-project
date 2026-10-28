using BackendApi.DTOs.Analysis;
using BackendApi.Helpers;
using BackendApi.Services.Transit;
using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;

namespace BackendApi.Services.Analysis;

public sealed class ThematicMapService : IThematicMapService
{
    private const int    RayCount        = 16;
    private const int    MinMinutesThreshold   = 1;
    private const int    MaxMinutesThreshold   = 180;
    private const double AssumedMaxRoadSpeedKmh = 110;
    private const double RadiusSafetyMargin     = 1.3;
    private static readonly int[] MinuteThresholds = [5, 10, 15];
    private const double TrafficRealismFactor = 1.6;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    private readonly IOsrmService _osrmService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ThematicMapService> _logger;

    private static readonly GeometryFactory GeometryFactory = new(new PrecisionModel(), 4326);

    public ThematicMapService(IOsrmService osrmService, IMemoryCache cache, ILogger<ThematicMapService> logger)
    {
        _osrmService = osrmService;
        _cache       = cache;
        _logger      = logger;
    }

    public async Task<IsochroneResultDto> GetIsochroneAsync(
        double centerLon, double centerLat, IReadOnlyList<int> minutesThresholds)
    {
        if (minutesThresholds.Count == 0)
            throw new ArgumentException("En az bir süre eşiği belirtilmelidir.");

        if (minutesThresholds.Any(m => m < MinMinutesThreshold || m > MaxMinutesThreshold))
            throw new ArgumentException($"Süre eşikleri {MinMinutesThreshold}-{MaxMinutesThreshold} dakika arasında olmalıdır.");

        // 5 decimals is ~1 m: clicks that close share the same 16 OSRM calls.
        var cacheKey = FormattableString.Invariant(
            $"isochrone:{Math.Round(centerLon, 5)}:{Math.Round(centerLat, 5)}:{string.Join(",", minutesThresholds.Distinct().Order())}");
        if (_cache.TryGetValue(cacheKey, out IsochroneResultDto? cached) && cached is not null)
            return cached;

        var center = new Coordinate(centerLon, centerLat);
        var maxRequestedMinutes = minutesThresholds.Max();
        var maxRadiusMeters = ComputeSafeRadiusMeters(maxRequestedMinutes);

        var rayTargets = BuildRayTargets(center, maxRadiusMeters);

        var rayResults = await Task.WhenAll(rayTargets.Select(target =>
            _osrmService.GetRouteWithCumulativeDurationsAsync(center, target)));

        var bands = new List<IsochroneBandDto>();

        // Büyükten küçüğe: frontend'in dış-içe doğru katmanlama sırasını garanti ediyoruz.
        foreach (var minutes in minutesThresholds.Distinct().OrderByDescending(m => m))
        {
            var targetSeconds = (minutes * 60.0) / TrafficRealismFactor;
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

            if (ringPoints.Count < 3) continue; // poligon oluşturmak için en az 3 nokta gerekli

            ringPoints.Add(ringPoints[0]); // poligon halkasını kapat
            // Curved routes can end out of angular order, so the ring may cross itself; fix it to a valid (multi)polygon.
            var polygon = GeometryFixer.Fix(GeometryFactory.CreatePolygon(ringPoints.ToArray()));
            if (polygon.IsEmpty) continue; // collinear ray ends collapse to nothing

            bands.Add(new IsochroneBandDto(minutes, GeometryConverter.ToWkt(polygon)));
        }

        var result = new IsochroneResultDto(bands);

        // Cache only complete results, so a temporary OSRM failure is not remembered for an hour.
        if (rayResults.All(r => r.Success))
            _cache.Set(cacheKey, result, CacheDuration);

        return result;
    }

    private static double ComputeSafeRadiusMeters(int maxMinutes)
    {
        var metersPerSecond = (AssumedMaxRoadSpeedKmh * 1000) / 3600.0;
        return maxMinutes * 60.0 * metersPerSecond * RadiusSafetyMargin;
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

        private static List<Coordinate> BuildRayTargets(Coordinate center, double radiusMeters)
    {
        var targets = new List<Coordinate>();
        var latRadians = center.Y * Math.PI / 180;

        var metersPerDegreeLat = 111_320.0;
        var metersPerDegreeLon = 111_320.0 * Math.Cos(latRadians);

        for (var i = 0; i < RayCount; i++)
        {
            var angle = 2 * Math.PI * i / RayCount;
            var dx = radiusMeters * Math.Cos(angle);
            var dy = radiusMeters * Math.Sin(angle);

            targets.Add(new Coordinate(
                center.X + dx / metersPerDegreeLon,
                center.Y + dy / metersPerDegreeLat));
        }

        return targets;
    }
}