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

public sealed record CategoryCountDto(
    int    CategoryId,
    string CategoryName,
    int    Count);

// Counts are cumulative: a band holds everything reachable within MinutesThreshold, not a ring.
public sealed record IsochroneAnalysisBandDto(
    int                             MinutesThreshold,
    string                          PolygonWkt,
    int                             StopCount,
    IReadOnlyList<CategoryCountDto> CategoryCounts);

public sealed record IsochroneAnalysisResultDto(
    IReadOnlyList<IsochroneAnalysisBandDto> Bands);
