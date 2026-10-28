using BackendApi.Services.Poi;

namespace BackendApi.Tests.Infrastructure;

// Replaces the public Overpass API: returns whatever the test put into Elements and records the query.
public sealed class FakeOverpassClient : IOverpassClient
{
    public IReadOnlyList<OsmElement> Elements  { get; set; } = [];
    public Exception?                Failure   { get; set; }
    public string?                   LastQuery { get; private set; }

    public void Reset()
    {
        Elements  = [];
        Failure   = null;
        LastQuery = null;
    }

    public Task<IReadOnlyList<OsmElement>> RunQueryAsync(string query, CancellationToken cancellationToken)
    {
        LastQuery = query;
        return Failure is null ? Task.FromResult(Elements) : Task.FromException<IReadOnlyList<OsmElement>>(Failure);
    }
}
