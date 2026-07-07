using Microsoft.AspNetCore.Authorization;

namespace BackendApi.Authorization;

public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permissionName)
        : base(permissionName)
    {
    }
}