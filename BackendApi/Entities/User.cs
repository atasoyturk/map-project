namespace BackendApi.Entities;

public sealed class User : BaseEntity
{
    public string Email        { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string Role         { get; init; } = string.Empty;

    public ICollection<UserRole>       UserRoles       { get; init; } = [];
    public ICollection<UserPermission> UserPermissions { get; init; } = [];
}