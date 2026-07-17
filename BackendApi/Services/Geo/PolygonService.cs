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

    public async Task<PolygonResponseDto> SaveAsync(GeoRequestDto request, int userId, int? teamId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);
        var isAdmin = roles.Contains("Admin");


        if (!isAdmin && !await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana çizim yapma yetkiniz bulunmamaktadır");

        var entity = new PolygonEntity
        {
            Geometry = geometry,
            Name     = request.Name,
            Color    = request.Color,
            UserId   = userId,
            TeamId   = teamId
        };

        _context.Polygons.Add(entity);
        await _context.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<IEnumerable<PolygonResponseDto>> GetAllAsync(int userId, int? teamId, GeoViewMode viewMode)
    {
        IQueryable<PolygonEntity> query = _context.Polygons.Where(p => !p.IsDeleted);

        query = viewMode switch
        {
            GeoViewMode.All  => query,
            GeoViewMode.Team => query.Where(p => p.TeamId == teamId),
            _                => query.Where(p => p.UserId == userId)
        };

        var entities = await query.ToListAsync();
        return entities.Select(ToDto);
    }

    public async Task<PolygonResponseDto?> UpdateAsync(int id, GeoRequestDto request, int userId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);
        var isAdmin = roles.Contains("Admin");

        if (!isAdmin && !await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana çizim yapma yetkiniz bulunmamaktadır");

        var entity = await _context.Polygons
            .FirstOrDefaultAsync(p => (isAdmin || p.UserId == userId) && p.Id == id && !p.IsDeleted);

        if (entity is null) return null;

        entity.Geometry       = geometry;
        entity.Name           = request.Name;
        entity.Color          = request.Color;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;

        await _context.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, int userId, IEnumerable<string> roles)
    {
        var isAdmin = roles.Contains("Admin");
        var entity = await _context.Polygons
            .FirstOrDefaultAsync(p => (isAdmin || p.UserId == userId) && p.Id == id && !p.IsDeleted);

        if (entity is null) return false;

        entity.IsDeleted      = true;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PolygonResponseDto?> GetByIdAsync(int id, int userId, IEnumerable<string> roles)
    {
        var isAdmin = roles.Contains("Admin");
        var entity = await _context.Polygons
            .FirstOrDefaultAsync(p => (isAdmin || p.UserId == userId) && p.Id == id && !p.IsDeleted);

        return entity is null ? null : ToDto(entity);
    }

    private static PolygonResponseDto ToDto(PolygonEntity entity) =>
        new(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry), 0, entity.CreatedDate);
}