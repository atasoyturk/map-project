using BackendApi.Data;
using BackendApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services;

public sealed class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;

    public PermissionService(AppDbContext context) => _context = context;

    public async Task<IList<EffectivePermissionDto>> GetEffectivePermissionsAsync(int userId)
    {
        
        var allPermissions = await _context.Permissions.ToListAsync();

        // users with permissions through roles
        var rolePermissions = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .SelectMany(ur => ur.Role.RolePermissions.Select(rp => new
            {
                PermissionId   = rp.PermissionId,
                PermissionName = rp.Permission.Name,
                RoleName       = ur.Role.Name
            }))
            .ToListAsync();

        // directly assigned user permissions
        var userPermissions = await _context.UserPermissions
            .Where(up => up.UserId == userId)
            .Include(up => up.Permission)
            .Select(up => up.PermissionId)
            .ToListAsync();

        // Combine both sources to determine effective permissions
        var result = allPermissions.Select(p =>
        {
            var fromRole = rolePermissions.FirstOrDefault(rp => rp.PermissionId == p.Id);
            var fromUser = userPermissions.Contains(p.Id);
            var isGranted = fromRole is not null || fromUser;

            return new EffectivePermissionDto(
                Name:        p.Name,
                Description: p.Description,
                IsGranted:   isGranted,
                Origin:      fromUser ? "User" : (fromRole is not null ? "Role" : "None"),
                RoleName:    fromUser ? null : fromRole?.RoleName
            );
        }).ToList();

        return result;
    }

    public async Task<bool> HasPermissionAsync(int userId, string permissionName)
    {
        
        var hasViaRole = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .AnyAsync(ur => ur.Role.RolePermissions
                .Any(rp => rp.Permission.Name == permissionName));

        if (hasViaRole) return true;

        
        return await _context.UserPermissions
            .Include(up => up.Permission)
            .AnyAsync(up => up.UserId == userId &&
                           up.Permission.Name == permissionName);
    }
}