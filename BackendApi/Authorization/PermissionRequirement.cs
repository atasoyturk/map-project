using Microsoft.AspNetCore.Authorization;

namespace BackendApi.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionName { get; }

    public PermissionRequirement(string permissionName)
        => PermissionName = permissionName;
}