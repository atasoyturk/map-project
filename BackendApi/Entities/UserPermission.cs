namespace BackendApi.Entities;

public sealed class UserPermission
{
    public int        UserId       { get; init; }
    public int        PermissionId { get; init; }
    public User       User         { get; init; } = null!;
    public Permission Permission   { get; init; } = null!;
}