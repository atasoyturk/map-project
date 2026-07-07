using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs;

public sealed class RegisterRequestDto
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; init; } = string.Empty;
}