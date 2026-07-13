using BackendApi.Data;
using BackendApi.DTOs.Geo;
using BackendApi.Entities.Geo;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Geo;

public sealed class LineService : ILineService
{
    private readonly AppDbContext _context;
    private readonly IGeoPermissionService _geoPermissionService;

    public LineService(AppDbContext context, IGeoPermissionService geoPermissionService)
    {
        _context              = context;
        _geoPermissionService = geoPermissionService;
    }

    public async Task<LineResponseDto> SaveAsync(GeoRequestDto request, int userId, int? teamId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana çizim yapma yetkiniz bulunmamaktadır");

        var entity = new LineEntity
        {
            Geometry = geometry,
            Name     = request.Name,
            Color    = request.Color,
            UserId   = userId,
            TeamId   = teamId
        };

        _context.Lines.Add(entity);
        await _context.SaveChangesAsync();

        return new LineResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(geometry), entity.CreatedDate);
    }

    public async Task<IEnumerable<LineResponseDto>> GetAllAsync(int userId, int? teamId, GeoViewMode viewMode)
    {
        IQueryable<LineEntity> query = _context.Lines.Where(l => !l.IsDeleted);

        query = viewMode switch
        {
            GeoViewMode.All  => query,
            GeoViewMode.Team => query.Where(l => l.TeamId == teamId),
            _                => query.Where(l => l.UserId == userId)   // Own
        };

        return await query
            .Select(l => new LineResponseDto(l.Id, l.Name, l.Color,
                GeometryConverter.ToWkt(l.Geometry), l.CreatedDate))
            .ToListAsync();
    }

    public async Task<LineResponseDto?> UpdateAsync(int id, GeoRequestDto request, int userId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana çizim yapma yetkiniz bulunmamaktadır");

        var entity = await _context.Lines
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId && !l.IsDeleted);

        if (entity is null) return null;

        entity.Geometry       = geometry;
        entity.Name           = request.Name;
        entity.Color          = request.Color;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;

        await _context.SaveChangesAsync();

        return new LineResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry), entity.CreatedDate);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.Lines
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId && !l.IsDeleted);

        if (entity is null) return false;

        entity.IsDeleted      = true;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LineResponseDto?> GetByIdAsync(int id, int userId)
    {
        var entity = await _context.Lines
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId && !l.IsDeleted);

        if (entity is null) return null;

        return new LineResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry), entity.CreatedDate);
    }
}