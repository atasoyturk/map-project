using BackendApi.Data;
using BackendApi.DTOs.Geo;
using BackendApi.Helpers;
using BackendApi.DTOs.Analysis;
using BackendApi.Entities.Poi;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Analysis;

public sealed class AnalysisService : IAnalysisService
{
    private readonly AppDbContext _context;
    private readonly IThematicMapService _thematicMapService;

    public AnalysisService(AppDbContext context, IThematicMapService thematicMapService)
    {
        _context            = context;
        _thematicMapService = thematicMapService;
    }

    public async Task<int> TempInventoryAsync(GeoRequestDto request, int userId)
    {
        var geometry  = GeometryConverter.FromWkt(request.WktGeometry);

        var points   = await _context.Points  .CountAsync(p => p.UserId == userId && !p.IsDeleted && p.IsActive && geometry.Intersects(p.Geometry));
        var lines    = await _context.Lines   .CountAsync(l => l.UserId == userId && !l.IsDeleted && l.IsActive && geometry.Intersects(l.Geometry));
        var polygons = await _context.Polygons.CountAsync(p => p.UserId == userId && !p.IsDeleted && p.IsActive && geometry.Intersects(p.Geometry));

        return points + lines + polygons;
    }

    public async Task<LocationAnalysisResponseDto> LocationAnalysisAsync(LocationAnalysisRequestDto request, int userId)
    {
        if (request.Criteria.Sum(c => c.Score) != 100)
            throw new ArgumentException("Kriter puanlarının toplamı 100 olmalıdır.");

        if (request.Criteria.Count < 2 || request.Criteria.Count > 5)
            throw new ArgumentException("En az 2, en fazla 5 kriter seçilmelidir.");

        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        var allCategories = await _context.PoiCategories.Where(c => !c.IsDeleted).ToListAsync();

        // Kategori ve tüm alt kategorilerini bul
        var criteriaCategoryIds = request.Criteria
            .Select(c => (Criterion: c, CategoryIds: GetCategoryAndDescendants(c.CategoryId, allCategories)))
            .ToList();
        var allCategoryIds = criteriaCategoryIds.SelectMany(c => c.CategoryIds).Distinct().ToList();

        var filteredPois = await _context.Pois
            .Where(p => !p.IsDeleted && p.IsActive && allCategoryIds.Contains(p.CategoryId) && geometry.Intersects(p.Geometry))
            .Select(p => new { p.Geometry.X, p.Geometry.Y, p.CategoryId })
            .ToListAsync();

        var heatmapPoints = new List<HeatmapPointDto>();

        foreach (var (criterion, categoryIds) in criteriaCategoryIds)
        {
            var categoryPois = filteredPois.Where(p => categoryIds.Contains(p.CategoryId));

            foreach (var poi in categoryPois)
            {
                heatmapPoints.Add(new HeatmapPointDto
                {
                    Latitude = poi.Y,
                    Longitude = poi.X,
                    Weight = criterion.Score / 100.0
                });
            }
        }

        return new LocationAnalysisResponseDto { HeatmapPoints = heatmapPoints };
    }

    public async Task<IsochroneAnalysisResultDto> IsochroneAnalysisAsync(IsochroneRequestDto request)
    {
        var isochrone = await _thematicMapService.GetIsochroneAsync(
            request.CenterLon, request.CenterLat, request.MinutesThresholds);

        var categoryNames = await _context.PoiCategories
            .Where(c => !c.IsDeleted)
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        var bands = new List<IsochroneAnalysisBandDto>();

        // Each band polygon covers the whole area reachable within its threshold, so counts are cumulative.
        foreach (var band in isochrone.Bands)
        {
            var polygon = GeometryConverter.FromWkt(band.PolygonWkt);

            var countsByCategory = await _context.Pois
                .Where(p => !p.IsDeleted && p.IsActive && polygon.Intersects(p.Geometry))
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToListAsync();

            var categoryCounts = countsByCategory
                .Where(c => categoryNames.ContainsKey(c.CategoryId))
                .Select(c => new CategoryCountDto(c.CategoryId, categoryNames[c.CategoryId], c.Count))
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.CategoryName)
                .ToList();

            var stopCount = await _context.TransitStops
                .CountAsync(s => !s.IsDeleted && s.IsActive && polygon.Intersects(s.Geometry));

            bands.Add(new IsochroneAnalysisBandDto(band.MinutesThreshold, band.PolygonWkt, stopCount, categoryCounts));
        }

        return new IsochroneAnalysisResultDto(bands);
    }

    private List<int> GetCategoryAndDescendants(int parentId, List<PoiCategory> allCategories)
    {
        var result = new List<int> { parentId };
        var children = allCategories.Where(c => c.ParentCategoryId == parentId).Select(c => c.Id).ToList();
        
        foreach (var childId in children)
        {
            result.AddRange(GetCategoryAndDescendants(childId, allCategories));
        }
        
        return result.Distinct().ToList();
    }

}