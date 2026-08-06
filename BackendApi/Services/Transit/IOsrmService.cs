using NetTopologySuite.Geometries;

namespace BackendApi.Services.Transit;

public interface IOsrmService
{
    Task<(bool Success, LineString? RouteGeometry, double? DurationSeconds, string? Error)>
        GetRouteAsync(IEnumerable<Coordinate> orderedCoordinates);

    Task<(bool Success, IReadOnlyList<Coordinate>? Coordinates, IReadOnlyList<double>? CumulativeDurations, string? Error)>
        GetRouteWithCumulativeDurationsAsync(Coordinate from, Coordinate to);
}