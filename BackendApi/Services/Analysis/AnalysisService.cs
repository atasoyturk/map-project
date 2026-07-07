using BackendApi.Data;
using BackendApi.DTOs.Geo;
using BackendApi.Helpers;
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
}