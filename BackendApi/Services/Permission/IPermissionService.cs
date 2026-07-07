using BackendApi.DTOs;

namespace BackendApi.Services;

public interface IPermissionService
{
    Task<IList<EffectivePermissionDto>> GetEffectivePermissionsAsync(int userId);
    Task<bool>                          HasPermissionAsync(int userId, string permissionName);
}