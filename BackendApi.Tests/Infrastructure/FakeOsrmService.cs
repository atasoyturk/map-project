using BackendApi.Services.Transit;
using NetTopologySuite.Geometries;

namespace BackendApi.Tests.Infrastructure;

// Replaces the public OSRM server: returns a straight line through the given coordinates.
// Cumulative-duration routes fail unless a test sets StraightLineDurationSeconds.
public sealed class FakeOsrmService : IOsrmService
{
    private int _durationCallCount;

    // When set, from→to is a straight line that takes this many seconds end to end.
    public double? StraightLineDurationSeconds { get; set; }
    // When set, the straight line ends at RayEnd(from, to) instead of to.
    public Func<Coordinate, Coordinate, Coordinate>? RayEnd { get; set; }
    public int     DurationCallCount => _durationCallCount;

    public void Reset()
    {
        StraightLineDurationSeconds = null;
        RayEnd                      = null;
        _durationCallCount          = 0;
    }

    public Task<(bool Success, LineString? RouteGeometry, double? DurationSeconds, string? Error)>
        GetRouteAsync(IEnumerable<Coordinate> orderedCoordinates)
    {
        var line = new LineString(orderedCoordinates.ToArray()) { SRID = 4326 };
        return Task.FromResult<(bool, LineString?, double?, string?)>((true, line, 0, null));
    }

    public Task<(bool Success, IReadOnlyList<Coordinate>? Coordinates, IReadOnlyList<double>? CumulativeDurations, string? Error)>
        GetRouteWithCumulativeDurationsAsync(Coordinate from, Coordinate to)
    {
        Interlocked.Increment(ref _durationCallCount);

        return Task.FromResult<(bool, IReadOnlyList<Coordinate>?, IReadOnlyList<double>?, string?)>(
            StraightLineDurationSeconds is { } seconds
                ? (true, [from, RayEnd?.Invoke(from, to) ?? to], [0, seconds], null)
                : (false, null, null, "OSRM is not available in tests."));
    }
}
