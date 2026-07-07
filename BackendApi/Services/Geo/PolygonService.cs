using BackendApi.Data;
using BackendApi.DTOs.Geo;
using BackendApi.Entities.Geo;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Geo;

public sealed class PolygonService : IPolygonService
{
    private readonly AppDbContext          _context;
    private readonly IGeoPermissionService _geoPermissionService;

    public PolygonService(AppDbContext context, IGeoPermissionService geoPermissionService)
    {
        _context              = context;
        _geoPermissionService = geoPermissionService;
    }

    public async Task<PolygonResponseDto> SaveAsync(GeoRequestDto request, int userId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alan dışında çizim yapma yetkiniz bulunmamaktadır.");

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

    public async Task<PolygonResponseDto?> UpdateAsync(int id, GeoRequestDto request, int userId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alan dışında çizim yapma yetkiniz bulunmamaktadır.");

        var entity = await _context.Polygons
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return null;

        entity.Geometry       = geometry;
        entity.Name           = request.Name;
        entity.Color          = request.Color;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;

        await _context.SaveChangesAsync();

        return new PolygonResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry), 0, entity.CreatedDate);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.Polygons
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return false;

        entity.IsDeleted      = true;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PolygonResponseDto?> GetByIdAsync(int id, int userId)
    {
        var entity = await _context.Polygons
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return null;

        return new PolygonResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry), 0, entity.CreatedDate);
    }
}