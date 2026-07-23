using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Company;

public sealed record CompanyRequestDto(
    [Required, MaxLength(150)] string Name,
    [Required] int CompanyCategoryId);

public sealed record CompanyResponseDto(
    int    Id,
    string Name,
    int    CompanyCategoryId,
    string CompanyCategoryName);

public sealed record AssignRouteToCompanyDto(
    [Required] int TransitRouteId);