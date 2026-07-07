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
            GeometryConverter.ToWkt(geometry), 0, entity.CreatedDate);
    }

    public async Task<IEnumerable<PolygonResponseDto>> GetAllAsync(int userId) =>
        await _context.Polygons
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .Select(p => new PolygonResponseDto(p.Id, p.Name, p.Color,
                GeometryConverter.ToWkt(p.Geometry), 0, p.CreatedDate))
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
        entity.ModifiedUserId  = userId;

        await _context.SaveChangesAsync();

        return new PolygonResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry), 0, entity.CreatedDate);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.Polygons
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return false;

        entity.IsDeleted    = true;
        entity.ModifiedDate = DateTime.UtcNow;
        entity.ModifiedUserId  = userId;
        await _context.SaveChangesAsync();
        return true;
    }
}