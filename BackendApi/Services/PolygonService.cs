using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Entities;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services;

public sealed class PolygonService : IPolygonService
{
    private readonly AppDbContext _context;

    public PolygonService(AppDbContext context) => _context = context;

    public async Task<PolygonResponseDto> SaveAsync(GeoRequestDto request)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        // Spatial intersection analysis
        var allPoints = await _context.Points.ToListAsync();
        var intersectedCount = allPoints.Count(p => geometry.Contains(p.Geometry));

        var entity = new PolygonEntity
        {
            Geometry = geometry,
            Name     = request.Name,
            Color    = request.Color
        };

        _context.Polygons.Add(entity);
        await _context.SaveChangesAsync();

        return new PolygonResponseDto(
            entity.Id,
            entity.Name,
            entity.Color,
            GeometryConverter.ToWkt(geometry),
            intersectedCount
        );
    }

    public async Task<IEnumerable<PolygonResponseDto>> GetAllAsync() =>
    await _context.Polygons
        .Select(p => new PolygonResponseDto(
            p.Id, p.Name, p.Color,
            GeometryConverter.ToWkt(p.Geometry), 0))
        .ToListAsync();
}