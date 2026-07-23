// BackendApi/DTOs/Company/VehicleDtos.cs
using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Company;

public sealed record VehicleRequestDto(
    [Required, MaxLength(20)] string PlateNumber,
    [Required] int CompanyId);

public sealed record VehicleResponseDto(
    int    Id,
    string PlateNumber,
    int    CompanyId,
    string CompanyName);