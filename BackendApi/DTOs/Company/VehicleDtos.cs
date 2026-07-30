using System.ComponentModel.DataAnnotations;
using BackendApi.Entities.Company;

namespace BackendApi.DTOs.Company;

public sealed record VehicleRequestDto(
    [Required, MaxLength(20)] string PlateNumber,
    [Required] int CompanyId,
    [Required] VehicleType VehicleType);

public sealed record VehicleResponseDto(
    int         Id,
    string      PlateNumber,
    int         CompanyId,
    string      CompanyName,
    VehicleType VehicleType);