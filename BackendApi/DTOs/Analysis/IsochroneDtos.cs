using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Analysis;

public sealed record IsochroneRequestDto(
    [Required] double CenterLon,
    [Required] double CenterLat,
    [Required, MinLength(1), MaxLength(10)] int[] MinutesThresholds);

public sealed record IsochroneBandDto(
    int    MinutesThreshold,
    string PolygonWkt);

public sealed record IsochroneResultDto(
    IReadOnlyList<IsochroneBandDto> Bands);