using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Entities;
using BackendApi.Entities.Auth;
using BackendApi.Services.Permission;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Admin;

public sealed class AdminService : IAdminService
{
    private readonly AppDbContext       _context;
    private readonly IPermissionService _permissionService;

    public AdminService(AppDbContext context, IPermissionService permissionService)
    {
        _context           = context;
        _permissionService = permissionService;
    }

    public async Task<IList<UserListDto>> GetAllUsersAsync() =>
        await _context.Users
            .Where(u => !u.IsDeleted)
            .Select(u => new UserListDto(
                u.Id,
                u.Email,
                u.IsActive,
                u.UserRoles.Select(ur => ur.Role.Name)))
            .ToListAsync();

    public async Task<bool> SetUserActiveAsync(int userId, bool isActive)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        if (user is null) return false;
        user.IsActive     = isActive;
        user.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
    {
        var exists = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (exists) return true;  // idempotent

        _context.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (userRole is null) return false;
        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IList<RoleDto>> GetAllRolesAsync() =>
    await _context.Roles
        .Select(r => new RoleDto(r.Id, r.Name))
        .ToListAsync();

    public async Task<bool> AssignPermissionToUserAsync(int userId, int permissionId)
    {
        var exists = await _context.UserPermissions
            .AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId);
        if (exists) return true;  // idempotent

        _context.UserPermissions.Add(new UserPermission { UserId = userId, PermissionId = permissionId });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemovePermissionFromUserAsync(int userId, int permissionId)
    {
        var userPermission = await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);
        if (userPermission is null) return false;
        _context.UserPermissions.Remove(userPermission);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IList<EffectivePermissionDto>> GetUserPermissionsAsync(int userId) =>
        await _permissionService.GetEffectivePermissionsAsync(userId);

    public async Task<RoleDto> CreateRoleAsync(string name)
    {
        var role = new Role { Name = name };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return new RoleDto(role.Id, role.Name);
    }

    public async Task<bool> DeleteRoleAsync(int roleId)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
        if (role is null) return false;
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }
}