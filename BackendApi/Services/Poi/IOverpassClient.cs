namespace BackendApi.Services.Poi;

// One OSM element from an Overpass `out center` response. Ways/relations carry their center as Lat/Lon.
public sealed record OsmElement(string Type, long Id, double Lat, double Lon, IReadOnlyDictionary<string, string> Tags);

public interface IOverpassClient
{
    Task<IReadOnlyList<OsmElement>> RunQueryAsync(string query, CancellationToken cancellationToken);
}
