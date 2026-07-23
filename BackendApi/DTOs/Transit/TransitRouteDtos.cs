using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Transit;

public sealed record TransitRouteRequestDto(
    [Required, MaxLength(100)] string Name,
    [Required, MaxLength(20)]  string Color);

public sealed record TransitRouteResponseDto(
    int      Id,
    string   Name,
    string   Color,
    int?     UserId,
    DateTime CreatedDate,
    string?  RouteWktGeometry);

public sealed record TransitRouteDetailDto(
    int      Id,
    string   Name,
    string   Color,
    int?     UserId,
    DateTime CreatedDate,
    string?  RouteWktGeometry,
    IList<TransitStopResponseDto> Stops);

public sealed record ReorderStopsDto(
    [Required] int[] StopIdsInOrder);