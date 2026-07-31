using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Transit;

public sealed record TripPlanRequestDto(
    [Required] double StartLon,
    [Required] double StartLat,
    [Required] double EndLon,
    [Required] double EndLat);

public sealed record TripLegDto(
    string  Type,              // "Walk" | "Bus"
    double  DurationSeconds,
    double  DistanceMeters,
    string  FromName,
    string  ToName,
    string  PathWkt,           // Haritada çizilecek geometri
    int?    RouteId,
    string? RouteName,
    string? RouteColor);

public sealed record TripPlanResultDto(
    double               TotalDurationSeconds,
    double               TotalWalkingMeters,
    IReadOnlyList<TripLegDto> Legs);