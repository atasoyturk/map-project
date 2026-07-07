namespace BackendApi.Entities;

public sealed class Permission
{
    public int    Id          { get; init; }
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; init; } = [];
    public ICollection<UserPermission> UserPermissions { get; init; } = [];
}