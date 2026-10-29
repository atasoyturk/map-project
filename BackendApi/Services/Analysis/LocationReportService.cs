using BackendApi.Data;
using BackendApi.DTOs.Analysis;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Analysis;

public sealed class LocationReportService : ILocationReportService
{
    private static readonly int[] BandMinutes = [5, 10, 15];
    private const int NearestCount = 3;
    // Planar-nearest (KNN) candidates fetched before re-ranking by true distance.
    private const int KnnCandidateCount = 10;

    private readonly AppDbContext        _context;
    private readonly IThematicMapService _thematicMapService;

    public LocationReportService(AppDbContext context, IThematicMapService thematicMapService)
    {
        _context            = context;
        _thematicMapService = thematicMapService;
    }

    public async Task<LocationReportDto> GetReportAsync(LocationReportRequestDto request)
    {
        var categories = await _context.PoiCategories
            .Where(c => !c.IsDeleted)
            .Select(c => new { c.Id, c.Name, c.ParentCategoryId })
            .ToListAsync();
        var categoryNames = categories.ToDictionary(c => c.Id, c => c.Name);

        var requestedIds = request.ComplementCategoryIds.Append(request.CompetitorCategoryId);
        if (requestedIds.Any(id => !categoryNames.ContainsKey(id)))
            throw new ArgumentException("Seçilen kategori bulunamadı.");

        HashSet<int> WithDescendants(int rootId)
        {
            var result = new HashSet<int> { rootId };
            var queue  = new Queue<int>([rootId]);
            while (queue.TryDequeue(out var id))
                foreach (var child in categories.Where(c => c.ParentCategoryId == id))
                    if (result.Add(child.Id)) queue.Enqueue(child.Id);
            return result;
        }

        var competitorIds = WithDescendants(request.CompetitorCategoryId);
        var complementSets = request.ComplementCategoryIds.Distinct()
            .Select(id => (Id: id, Ids: WithDescendants(id)))
            .ToList();
        var complementIds = complementSets.SelectMany(s => s.Ids).ToHashSet();

        if (complementIds.Overlaps(competitorIds))
            throw new ArgumentException("Rakip kategori tamamlayıcı kategorilerle çakışamaz.");

        var isochrone = await _thematicMapService.GetIsochroneAsync(request.CenterLon, request.CenterLat, BandMinutes);
        if (isochrone.Bands.Count != BandMinutes.Length)
            throw new InvalidOperationException("Bu nokta için hizmet alanı hesaplanamadı (yol ağı bulunamadı).");

        var relevantIds = competitorIds.Concat(complementIds).ToList();
        var bands = new List<LocationReportBandDto>();

        foreach (var band in isochrone.Bands.OrderBy(b => b.MinutesThreshold))
        {
            var polygon = GeometryConverter.FromWkt(band.PolygonWkt);

            var counts = await _context.Pois
                .Where(p => !p.IsDeleted && p.IsActive && relevantIds.Contains(p.CategoryId) && polygon.Intersects(p.Geometry))
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.CategoryId, g => g.Count);

            int CountIn(IEnumerable<int> ids) => ids.Sum(id => counts.GetValueOrDefault(id));

            var stopCount = await _context.TransitStops
                .CountAsync(s => !s.IsDeleted && s.IsActive && polygon.Intersects(s.Geometry));

            var area = await _context.Database
                .SqlQuery<double>($"SELECT ST_Area(ST_GeomFromText({band.PolygonWkt}, 4326)::geography) AS \"Value\"")
                .SingleAsync();

            bands.Add(new LocationReportBandDto(
                band.MinutesThreshold, band.PolygonWkt, area,
                CountIn(competitorIds), CountIn(complementIds), stopCount,
                complementSets.Select(s => new CategoryCountDto(s.Id, categoryNames[s.Id], CountIn(s.Ids))).ToList()));
        }

        var nearestCompetitors = await NearestPoisAsync(request.CenterLon, request.CenterLat, competitorIds.ToArray());
        var nearestStops       = await NearestStopsAsync(request.CenterLon, request.CenterLat);

        var score = LocationScore.Compute(
            bands[0].ComplementCount, bands[1].ComplementCount, bands[2].ComplementCount,
            bands[1].CompetitorCount, nearestStops.FirstOrDefault()?.DistanceMeters, bands[1].StopCount);

        return new LocationReportDto(bands, nearestCompetitors, nearestStops, score);
    }

    // KNN (<->) on the GiST index picks planar-nearest candidates; they are then re-ranked by geography distance
    // (meters on the spheroid). Planar and true order can differ, hence more candidates than results.
    private Task<List<NearbyPlaceDto>> NearestPoisAsync(double lon, double lat, int[] categoryIds)
        => _context.Database.SqlQuery<NearbyPlaceDto>($"""
            SELECT c."Name", ST_Distance(c."Geometry"::geography, ST_SetSRID(ST_MakePoint({lon}, {lat}), 4326)::geography) AS "DistanceMeters"
            FROM (
                SELECT p."Name", p."Geometry"
                FROM tbl_poi p
                WHERE NOT p."IsDeleted" AND p."IsActive" AND p."CategoryId" = ANY({categoryIds})
                ORDER BY p."Geometry" <-> ST_SetSRID(ST_MakePoint({lon}, {lat}), 4326)
                LIMIT {KnnCandidateCount}
            ) c
            ORDER BY "DistanceMeters"
            LIMIT {NearestCount}
            """).ToListAsync();

    private Task<List<NearbyPlaceDto>> NearestStopsAsync(double lon, double lat)
        => _context.Database.SqlQuery<NearbyPlaceDto>($"""
            SELECT c."Name", ST_Distance(c."Geometry"::geography, ST_SetSRID(ST_MakePoint({lon}, {lat}), 4326)::geography) AS "DistanceMeters"
            FROM (
                SELECT s."Name", s."Geometry"
                FROM tbl_transit_stop s
                WHERE NOT s."IsDeleted" AND s."IsActive"
                ORDER BY s."Geometry" <-> ST_SetSRID(ST_MakePoint({lon}, {lat}), 4326)
                LIMIT {KnnCandidateCount}
            ) c
            ORDER BY "DistanceMeters"
            LIMIT {NearestCount}
            """).ToListAsync();
}
