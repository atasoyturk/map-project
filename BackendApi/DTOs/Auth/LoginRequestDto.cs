using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs;

public sealed class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(1)]
    public string Password { get; init; } = string.Empty;
}