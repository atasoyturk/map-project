using BackendApi.Services.Transit;
using NetTopologySuite.Geometries;

namespace BackendApi.Tests.Infrastructure;

// Replaces the public OSRM server: returns a straight line through the given coordinates.
public sealed class FakeOsrmService : IOsrmService
{
    public Task<(bool Success, LineString? RouteGeometry, double? DurationSeconds, string? Error)>
        GetRouteAsync(IEnumerable<Coordinate> orderedCoordinates)
    {
        var line = new LineString(orderedCoordinates.ToArray());
        return Task.FromResult<(bool, LineString?, double?, string?)>((true, line, 0, null));
    }

    public Task<(bool Success, IReadOnlyList<Coordinate>? Coordinates, IReadOnlyList<double>? CumulativeDurations, string? Error)>
        GetRouteWithCumulativeDurationsAsync(Coordinate from, Coordinate to)
        => Task.FromResult<(bool, IReadOnlyList<Coordinate>?, IReadOnlyList<double>?, string?)>(
            (false, null, null, "OSRM is not available in tests."));
}
