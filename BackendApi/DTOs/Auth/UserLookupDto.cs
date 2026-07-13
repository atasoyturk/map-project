namespace BackendApi.DTOs.Auth;

public sealed record UserLookupDto(int Id, string Email, string? TeamName);