using NetTopologySuite.Geometries;

namespace BackendApi.Services.Transit;

public sealed record SpeedSchedulePoint(double DistanceMeters, double ElapsedSeconds);

public sealed class RouteSimulationState
{
    public required int         RouteId         { get; init; }
    public required int         VehicleId       { get; init; }
    public required string      PlateNumber     { get; init; }
    public required int         StartedByUserId { get; init; }
    public required DateTime    StartedAtUtc    { get; init; }
    public required double      DurationSeconds { get; init; }
    public required LineString  RouteGeometry   { get; init; }
    public IReadOnlyList<SpeedSchedulePoint>? SpeedSchedule { get; init; }
}