namespace BackendApi.Entities.Auth;

public sealed class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<UserRole>       UserRoles       { get; init; } = [];
    public ICollection<RolePermission> RolePermissions { get; init; } = [];
}