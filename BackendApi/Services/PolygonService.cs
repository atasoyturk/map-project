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

    public async Task<PolygonResponseDto> SaveAsync(GeoRequestDto request, int userId)
    {
        var geometry  = GeometryConverter.FromWkt(request.WktGeometry);
        
        var points   = await _context.Points  .Where(p => p.UserId == userId && !p.IsDeleted && p.IsActive).ToListAsync();
        var lines    = await _context.Lines   .Where(l => l.UserId == userId && !l.IsDeleted && l.IsActive).ToListAsync();
        var polygons = await _context.Polygons.Where(p => p.UserId == userId && !p.IsDeleted && p.IsActive).ToListAsync();

        var intersectedCount =
            points  .Count(p => geometry.Intersects(p.Geometry)) +
            lines   .Count(l => geometry.Intersects(l.Geometry)) +
            polygons.Count(p => geometry.Intersects(p.Geometry));

        var entity = new PolygonEntity
        {
            Geometry = geometry,
            Name     = request.Name,
            Color    = request.Color,
            UserId   = userId
        };

        _context.Polygons.Add(entity);
        await _context.SaveChangesAsync();

        return new PolygonResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(geometry), intersectedCount);
    }

    public async Task<IEnumerable<PolygonResponseDto>> GetAllAsync(int userId) =>
        await _context.Polygons
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .Select(p => new PolygonResponseDto(p.Id, p.Name, p.Color,
                GeometryConverter.ToWkt(p.Geometry), 0))
            .ToListAsync();

    public async Task<PolygonResponseDto?> UpdateAsync(int id, GeoRequestDto request, int userId)
    {
        var entity = await _context.Polygons
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return null;

        entity.Geometry     = GeometryConverter.FromWkt(request.WktGeometry);
        entity.Name         = request.Name;
        entity.Color        = request.Color;
        entity.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new PolygonResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry), 0);
    }
}