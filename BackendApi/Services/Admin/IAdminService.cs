using BackendApi.DTOs.Admin;
using BackendApi.DTOs.Permission;


namespace BackendApi.Services.Admin;

public interface IAdminService
{
    Task<IList<UserListDto>>            GetAllUsersAsync();
    Task<IList<TeamDto>>                GetAllTeamsAsync();
    Task<bool>                          SetUserActiveAsync(int userId, bool isActive);
    Task<bool>                          AssignRoleToUserAsync(int userId, int roleId);
    Task<bool>                          AssignTeamToUserAsync(int userId, int? teamId);
    Task<int>                           AssignTeamToUsersAsync(int[] userIds, int? teamId);
    Task<bool>                          RemoveRoleFromUserAsync(int userId, int roleId);
    Task<IList<RoleDto>>                GetAllRolesAsync();
    Task<bool>                          AssignPermissionToUserAsync(int userId, int permissionId);
    Task<bool>                          RemovePermissionFromUserAsync(int userId, int permissionId);
    Task<IList<EffectivePermissionDto>> GetUserPermissionsAsync(int userId);
    Task<RoleDto>                       CreateRoleAsync(string name);
    Task<TeamDto>                       CreateTeamAsync(string name);
    Task<bool>                          DeleteRoleAsync(int roleId);
    Task<bool>                          DeleteTeamAsync(int teamId);
    Task<bool>                          CreateEmployeeAsync(string email, string password);

}

public sealed record RoleDto(int Id, string Name);