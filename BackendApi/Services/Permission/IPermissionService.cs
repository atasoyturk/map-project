using BackendApi.DTOs;

namespace BackendApi.Services.Permission;

public interface IPermissionService
{
    Task<IList<EffectivePermissionDto>> GetEffectivePermissionsAsync(int userId);
    Task<bool>                          HasPermissionAsync(int userId, string permissionName);
}