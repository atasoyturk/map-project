using BackendApi.Data;
using BackendApi.DTOs.Geo;
using BackendApi.Entities.Geo;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Geo;

public sealed class PointService : IPointService
{
    private readonly AppDbContext _context;
    private readonly IGeoPermissionService _geoPermissionService;
    public PointService(AppDbContext context, IGeoPermissionService geoPermissionService)
    {
        _context              = context;
        _geoPermissionService = geoPermissionService;
    }

    public async Task<PointResponseDto> SaveAsync(GeoRequestDto request, int userId, int? teamId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana çizim yapma yetkiniz bulunmamaktadır");

        var entity = new PointEntity
        {
            Geometry = geometry,
            Name     = request.Name,
            Color    = request.Color,
            UserId   = userId,
            TeamId   = teamId   
        };

        _context.Points.Add(entity);
        await _context.SaveChangesAsync();

        return new PointResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(geometry), entity.CreatedDate);
    }

    public async Task<IEnumerable<PointResponseDto>> GetAllAsync(int userId, int? teamId, GeoViewMode viewMode)
    {
        IQueryable<PointEntity> query = _context.Points.Where(p => !p.IsDeleted);

        query = viewMode switch
        {
            GeoViewMode.All  => query,
            GeoViewMode.Team => query.Where(p => p.TeamId == teamId),
            _                => query.Where(p => p.UserId == userId)   // Own
        };

        return await query
            .Select(p => new PointResponseDto(p.Id, p.Name, p.Color,
                GeometryConverter.ToWkt(p.Geometry), p.CreatedDate))
            .ToListAsync();
    }

    public async Task<PointResponseDto?> UpdateAsync(int id, GeoRequestDto request, int userId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana çizim yapma yetkiniz bulunmamaktadır");
    
        var entity = await _context.Points
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return null;

        entity.Geometry     = GeometryConverter.FromWkt(request.WktGeometry);
        entity.Name         = request.Name;
        entity.Color        = request.Color;
        entity.ModifiedDate = DateTime.UtcNow;
        entity.ModifiedUserId  = userId; 

        await _context.SaveChangesAsync();

        return new PointResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry), entity.CreatedDate);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.Points
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return false;

        entity.IsDeleted    = true;
        entity.ModifiedDate = DateTime.UtcNow;
        entity.ModifiedUserId  = userId; 
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PointResponseDto?> GetByIdAsync(int id, int userId)
    {
        var entity = await _context.Points
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return null;

        return new PointResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry), entity.CreatedDate);
    }
}