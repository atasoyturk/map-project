using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Transit;

public sealed record TransitStopRequestDto(
    [Required, MaxLength(150)] string Name,
    [Required] string WktGeometry,
    [Required] int    TransitRouteId);

public sealed record TransitStopResponseDto(
    int      Id,
    string   Name,
    string   WktGeometry,
    int      TransitRouteId,
    int      UserId,
    int      SortOrder,
    DateTime CreatedDate);