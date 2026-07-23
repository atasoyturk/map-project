using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Company;

public sealed record CompanyCategoryRequestDto(
    [Required, MaxLength(100)] string Name);

public sealed record CompanyCategoryResponseDto(
    int    Id,
    string Name);