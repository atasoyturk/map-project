using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services;

public sealed class AnalysisService : IAnalysisService
{
    private readonly AppDbContext _context;

    public AnalysisService(AppDbContext context) => _context = context;

    public async Task<int> TempInventoryAsync(GeoRequestDto request, int userId)
    {
        var geometry  = GeometryConverter.FromWkt(request.WktGeometry);
        var allPoints = await _context.Points
            .Where(p => p.UserId == userId && !p.IsDeleted && p.IsActive)
            .ToListAsync();

        return allPoints.Count(p => geometry.Intersects(p.Geometry));
    }
}