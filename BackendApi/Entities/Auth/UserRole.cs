namespace BackendApi.Entities;

public sealed class UserRole
{
    public int  UserId { get; init; }
    public int  RoleId { get; init; }
    public User User   { get; init; } = null!;
    public Role Role   { get; init; } = null!;
}