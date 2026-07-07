namespace BackendApi.Entities;

public sealed class RolePermission
{
    public int        RoleId       { get; init; }
    public int        PermissionId { get; init; }
    public Role       Role         { get; init; } = null!;
    public Permission Permission   { get; init; } = null!;
}