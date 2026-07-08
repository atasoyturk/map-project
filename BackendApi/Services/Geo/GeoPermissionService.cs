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

    // ── Boundary validation ───────────────────────────────────────────────────

    public async Task<bool> IsWithinBoundaryAsync(
        int                 userId,
        IEnumerable<string> roles,
        Geometry            geometry)
    {
        var userBoundaries = await _context.UserGeoPermissions
            .Where(ugp => ugp.UserId == userId)
            .Include(ugp => ugp.GeoPermission)
            .Where(ugp => ugp.GeoPermission.IsActive && !ugp.GeoPermission.IsDeleted)
            .Select(ugp => ugp.GeoPermission.BoundaryGeometry)
            .ToListAsync();

        var roleBoundaries = await _context.RoleGeoPermissions
            .Include(rgp => rgp.Role)
            .Include(rgp => rgp.GeoPermission)
            .Where(rgp => roles.Contains(rgp.Role.Name)
                       && rgp.GeoPermission.IsActive
                       && !rgp.GeoPermission.IsDeleted)
            .Select(rgp => rgp.GeoPermission.BoundaryGeometry)
            .ToListAsync();

        var allBoundaries = userBoundaries.Concat(roleBoundaries).ToList();

        if (allBoundaries.Count == 0) return true;

        return allBoundaries.Any(boundary => boundary.Contains(geometry));
    }

    // ── GeoPermission CRUD ────────────────────────────────────────────────────

    public async Task<GeoPermissionResponseDto> CreateAsync(GeoPermissionRequestDto request)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        var entity = new GeoPermissionEntity
        {
            Name             = request.Name,
            BoundaryGeometry = geometry,
            IsActive         = true,
        };

        _context.GeoPermissions.Add(entity);
        await _context.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<IEnumerable<GeoPermissionResponseDto>> GetAllAsync() =>
        await _context.GeoPermissions
            .Where(gp => !gp.IsDeleted)
            .Select(gp => new GeoPermissionResponseDto(
                gp.Id, gp.Name,
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

    // ── User assignment ───────────────────────────────────────────────────────

    public async Task<bool> AssignToUserAsync(int userId, int geoPermissionId)
    {
        var exists = await _context.UserGeoPermissions
            .AnyAsync(ugp => ugp.UserId == userId && ugp.GeoPermissionId == geoPermissionId);

        if (exists) return true; // idempotent

        _context.UserGeoPermissions.Add(new UserGeoPermission
        {
            UserId          = userId,
            GeoPermissionId = geoPermissionId,
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFromUserAsync(int userId, int geoPermissionId)
    {
        var entity = await _context.UserGeoPermissions
            .FirstOrDefaultAsync(ugp => ugp.UserId == userId && ugp.GeoPermissionId == geoPermissionId);

        if (entity is null) return false;

        _context.UserGeoPermissions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<GeoPermissionResponseDto>> GetByUserAsync(int userId) =>
        await _context.UserGeoPermissions
            .Where(ugp => ugp.UserId == userId)
            .Include(ugp => ugp.GeoPermission)
            .Where(ugp => !ugp.GeoPermission.IsDeleted)
            .Select(ugp => new GeoPermissionResponseDto(
                ugp.GeoPermission.Id,
                ugp.GeoPermission.Name,
                GeometryConverter.ToWkt(ugp.GeoPermission.BoundaryGeometry),
                ugp.GeoPermission.IsActive))
            .ToListAsync();

    // ── Role assignment ───────────────────────────────────────────────────────

    public async Task<bool> AssignToRoleAsync(int roleId, int geoPermissionId)
    {
        var exists = await _context.RoleGeoPermissions
            .AnyAsync(rgp => rgp.RoleId == roleId && rgp.GeoPermissionId == geoPermissionId);

        if (exists) return true; // idempotent

        _context.RoleGeoPermissions.Add(new RoleGeoPermission
        {
            RoleId          = roleId,
            GeoPermissionId = geoPermissionId,
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFromRoleAsync(int roleId, int geoPermissionId)
    {
        var entity = await _context.RoleGeoPermissions
            .FirstOrDefaultAsync(rgp => rgp.RoleId == roleId && rgp.GeoPermissionId == geoPermissionId);

        if (entity is null) return false;

        _context.RoleGeoPermissions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<GeoPermissionResponseDto>> GetByRoleAsync(int roleId) =>
        await _context.RoleGeoPermissions
            .Where(rgp => rgp.RoleId == roleId)
            .Include(rgp => rgp.GeoPermission)
            .Where(rgp => !rgp.GeoPermission.IsDeleted)
            .Select(rgp => new GeoPermissionResponseDto(
                rgp.GeoPermission.Id,
                rgp.GeoPermission.Name,
                GeometryConverter.ToWkt(rgp.GeoPermission.BoundaryGeometry),
                rgp.GeoPermission.IsActive))
            .ToListAsync();

    // ── Helper ────────────────────────────────────────────────────────────────

    private static GeoPermissionResponseDto ToDto(GeoPermissionEntity e) =>
        new(e.Id, e.Name, GeometryConverter.ToWkt(e.BoundaryGeometry), e.IsActive);
}