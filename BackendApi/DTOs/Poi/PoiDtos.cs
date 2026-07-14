using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Poi;

public sealed record PoiRequestDto(
    [Required, MaxLength(150)] string Name,
    [Required, MaxLength(200)] string WorkingHours,
    [Required] string WktGeometry,
    [Required] int    CategoryId);

public sealed record PoiResponseDto(
    int      Id,
    string   Name,
    string   WorkingHours,
    string   WktGeometry,
    int      CategoryId,
    int      UserId,
    DateTime CreatedDate);