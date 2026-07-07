namespace BackendApi.Entities;

public sealed class Role
{
    public int    Id   { get; init; }
    public string Name { get; set; } = string.Empty;

    public ICollection<UserRole>       UserRoles       { get; init; } = [];
    public ICollection<RolePermission> RolePermissions { get; init; } = [];
}