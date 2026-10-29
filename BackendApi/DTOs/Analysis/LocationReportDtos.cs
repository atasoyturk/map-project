using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Analysis;

public sealed record LocationReportRequestDto(
    [Range(-180, 180)] double CenterLon,
    [Range(-90, 90)]   double CenterLat,
    [Range(1, int.MaxValue)] int CompetitorCategoryId,
    [Required, MinLength(1), MaxLength(5)] int[] ComplementCategoryIds);

// Counts are cumulative: a band holds everything reachable within MinutesThreshold, not a ring.
public sealed record LocationReportBandDto(
    int                             MinutesThreshold,
    string                          PolygonWkt,
    double                          AreaSquareMeters,
    int                             CompetitorCount,
    int                             ComplementCount,
    int                             StopCount,
    IReadOnlyList<CategoryCountDto> ComplementCounts);

// Straight-line (as the crow flies) distance from the report center.
public sealed record NearbyPlaceDto(
    string Name,
    double DistanceMeters);

public sealed record LocationScoreDto(
    int    Total,
    int    ComplementScore,
    int    CompetitionScore,
    int    TransitScore,
    double ComplementWeight,
    double CompetitionWeight,
    double TransitWeight,
    double? SaturationRatio);

public sealed record LocationReportDto(
    IReadOnlyList<LocationReportBandDto> Bands,
    IReadOnlyList<NearbyPlaceDto>        NearestCompetitors,
    IReadOnlyList<NearbyPlaceDto>        NearestStops,
    LocationScoreDto                     Score);
