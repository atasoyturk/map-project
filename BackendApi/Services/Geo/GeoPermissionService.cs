using BackendApi.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace BackendApi.Services.Geo;

public sealed class GeoPermissionService : IGeoPermissionService
{
    private readonly AppDbContext _context;

    public GeoPermissionService(AppDbContext context) => _context = context;

    public async Task<bool> IsWithinBoundaryAsync(
        int                  userId,
        IEnumerable<string>  roles,
        Geometry             geometry)
    {
        // boundaries for the user
        var userBoundaries = await _context.GeoPermissions
            .Where(gp => gp.UserId == userId && !gp.IsDeleted && gp.IsActive)
            .Select(gp => gp.BoundaryGeometry)
            .ToListAsync();

        // boundaries for the roles
        var roleBoundaries = await _context.GeoPermissions
            .Where(gp => gp.RoleId != null && !gp.IsDeleted && gp.IsActive)
            .Include(gp => gp.Role)
            .Where(gp => roles.Contains(gp.Role!.Name))
            .Select(gp => gp.BoundaryGeometry)
            .ToListAsync();

        var allBoundaries = userBoundaries.Concat(roleBoundaries).ToList();

        // no boundaries
        if (allBoundaries.Count == 0) return true;

        // drawing geometry is within any of the boundaries ? 
        return allBoundaries.Any(boundary => boundary.Contains(geometry));
    }
}