using BackendApi.Data;
using BackendApi.DTOs.Geo;
using BackendApi.Entities.Geo;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace BackendApi.Services.Geo;

public sealed class GeoPermissionService : IGeoPermissionService
{
    private readonly AppDbContext _context;

    public GeoPermissionService(AppDbContext context) => _context = context;

    public async Task<bool> IsWithinBoundaryAsync(
        int                 userId,
        IEnumerable<string> roles,
        Geometry            geometry)
    {
        var userBoundaries = await _context.GeoPermissions
            .Where(gp => gp.UserId == userId && !gp.IsDeleted && gp.IsActive)
            .Select(gp => gp.BoundaryGeometry)
            .ToListAsync();

        var roleBoundaries = await _context.GeoPermissions
            .Where(gp => gp.RoleId != null && !gp.IsDeleted && gp.IsActive)
            .Include(gp => gp.Role)
            .Where(gp => roles.Contains(gp.Role!.Name))
            .Select(gp => gp.BoundaryGeometry)
            .ToListAsync();

        var allBoundaries = userBoundaries.Concat(roleBoundaries).ToList();

        if (allBoundaries.Count == 0) return true;

        return allBoundaries.Any(boundary => boundary.Contains(geometry));
    }

    public async Task<GeoPermissionResponseDto> CreateAsync(GeoPermissionRequestDto request)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        var entity = new GeoPermissionEntity
        {
            UserId           = request.UserId,
            RoleId           = request.RoleId,
            BoundaryGeometry = geometry,
            IsActive         = true,
        };

        _context.GeoPermissions.Add(entity);
        await _context.SaveChangesAsync();

        return new GeoPermissionResponseDto(
            entity.Id,
            entity.UserId,
            entity.RoleId,
            GeometryConverter.ToWkt(entity.BoundaryGeometry),
            entity.IsActive);
    }

    public async Task<IEnumerable<GeoPermissionResponseDto>> GetAllAsync() =>
        await _context.GeoPermissions
            .Where(gp => !gp.IsDeleted)
            .Select(gp => new GeoPermissionResponseDto(
                gp.Id,
                gp.UserId,
                gp.RoleId,
                GeometryConverter.ToWkt(gp.BoundaryGeometry),
                gp.IsActive))
            .ToListAsync();

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.GeoPermissions
            .FirstOrDefaultAsync(gp => gp.Id == id && !gp.IsDeleted);

        if (entity is null) return false;

        entity.IsDeleted    = true;
        entity.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}