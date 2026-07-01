namespace BackendApi.Entities;

public sealed class User
{
    public int    Id           { get; init; }
    public string Email        { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string Role         { get; init; } = string.Empty;

    public bool     IsActive      { get; set; } = true;
    public bool     IsDeleted     { get; set; } = false;
    public DateTime ModifiedDate  { get; set; } = DateTime.UtcNow;
}