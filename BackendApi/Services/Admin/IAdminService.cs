using BackendApi.DTOs;

namespace BackendApi.Services.Admin;

public interface IAdminService
{
    Task<IList<UserListDto>>            GetAllUsersAsync();
    Task<bool>                          SetUserActiveAsync(int userId, bool isActive);
    Task<bool>                          AssignRoleToUserAsync(int userId, int roleId);
    Task<bool>                          RemoveRoleFromUserAsync(int userId, int roleId);
    Task<IList<RoleDto>>                GetAllRolesAsync();
    Task<bool>                          AssignPermissionToUserAsync(int userId, int permissionId);
    Task<bool>                          RemovePermissionFromUserAsync(int userId, int permissionId);
    Task<IList<EffectivePermissionDto>> GetUserPermissionsAsync(int userId);
    Task<RoleDto>                       CreateRoleAsync(string name);
    Task<bool>                          DeleteRoleAsync(int roleId);
}

public sealed record RoleDto(int Id, string Name);