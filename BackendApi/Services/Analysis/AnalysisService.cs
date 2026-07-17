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

    public AnalysisService(AppDbContext context) => _context = context;

    public async Task<int> TempInventoryAsync(GeoRequestDto request, int userId)
    {
        var geometry  = GeometryConverter.FromWkt(request.WktGeometry);
        
        var points   = await _context.Points  .Where(p => p.UserId == userId && !p.IsDeleted && p.IsActive).ToListAsync();
        var lines    = await _context.Lines   .Where(l => l.UserId == userId && !l.IsDeleted && l.IsActive).ToListAsync();
        var polygons = await _context.Polygons.Where(p => p.UserId == userId && !p.IsDeleted && p.IsActive).ToListAsync();

        return
            points  .Count(p => geometry.Intersects(p.Geometry)) +
            lines   .Count(l => geometry.Intersects(l.Geometry)) +
            polygons.Count(p => geometry.Intersects(p.Geometry));
    }

    public async Task<LocationAnalysisResponseDto> LocationAnalysisAsync(LocationAnalysisRequestDto request, int userId)
    {
        if (request.Criteria.Sum(c => c.Score) != 100)
            throw new ArgumentException("Kriter puanlarının toplamı 100 olmalıdır.");

        if (request.Criteria.Count < 2 || request.Criteria.Count > 5)
            throw new ArgumentException("En az 2, en fazla 5 kriter seçilmelidir.");

        var geometry = GeometryConverter.FromWkt(request.WktGeometry);
        
        var poisInRegion = await _context.Pois
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        var filteredPois = poisInRegion
            .Where(p => geometry.Intersects(p.Geometry))
            .ToList();

        var allCategories = await _context.PoiCategories.Where(c => !c.IsDeleted).ToListAsync();
        
        var heatmapPoints = new List<HeatmapPointDto>();

        foreach (var criterion in request.Criteria)
        {
            // Kategori ve tüm alt kategorilerini bul
            var categoryIds = GetCategoryAndDescendants(criterion.CategoryId, allCategories);
            
            var categoryPois = filteredPois.Where(p => categoryIds.Contains(p.CategoryId));
            
            foreach (var poi in categoryPois)
            {
                heatmapPoints.Add(new HeatmapPointDto
                {
                    Latitude = poi.Geometry.Y,
                    Longitude = poi.Geometry.X,
                    Weight = criterion.Score / 100.0
                });
            }
        }

        return new LocationAnalysisResponseDto { HeatmapPoints = heatmapPoints };
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