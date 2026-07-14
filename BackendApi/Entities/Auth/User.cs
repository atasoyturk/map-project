namespace BackendApi.Entities.Auth;

public sealed class User : BaseEntity
{
    public string Email        { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public int?   TeamId       { get; set; }   

    public ICollection<UserRole>       UserRoles       { get; init; } = [];
    public ICollection<UserPermission> UserPermissions { get; init; } = [];
}