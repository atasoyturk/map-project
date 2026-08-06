using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Transit;

public sealed record IsochroneRequestDto(
    [Required] double CenterLon,
    [Required] double CenterLat);

public sealed record IsochroneBandDto(
    int    MinutesThreshold,
    string PolygonWkt);

public sealed record IsochroneResultDto(
    IReadOnlyList<IsochroneBandDto> Bands);