using System.Globalization;
using System.Text.Json;
using BackendApi.Settings;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace BackendApi.Services.Transit;

public sealed class OsrmService : IOsrmService
{
    private readonly IHttpClientFactory  _httpClientFactory;
    private readonly OsrmSettings        _settings;
    private readonly ILogger<OsrmService> _logger;

    private static readonly GeometryFactory GeometryFactory = new(new PrecisionModel(), 4326);

    public OsrmService(
        IHttpClientFactory      httpClientFactory,
        IOptions<OsrmSettings>  settings,
        ILogger<OsrmService>    logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings           = settings.Value;
        _logger             = logger;
    }

    public async Task<(bool Success, LineString? RouteGeometry, double? DurationSeconds, string? Error)>
        GetRouteAsync(IEnumerable<Coordinate> orderedCoordinates)
    {
        var coords = orderedCoordinates.ToList();

        if (coords.Count < 2)
            return (false, null, null, "Rota oluşturmak için en az 2 durak gereklidir.");

        try
        {
            var client = _httpClientFactory.CreateClient("Osrm");

            var coordinateSegment = string.Join(";", coords.Select(c =>
                $"{c.X.ToString(CultureInfo.InvariantCulture)},{c.Y.ToString(CultureInfo.InvariantCulture)}"));

            var url = $"{_settings.BaseUrl}/route/v1/{_settings.Profile}/{coordinateSegment}?overview=full&geometries=geojson";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OSRM route request failed: {StatusCode}", response.StatusCode);
                return (false, null, null, "OSRM servisinden rota alınamadı.");
            }

            var content = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(content);

            var code = json.RootElement.GetProperty("code").GetString();
            if (code != "Ok")
            {
                _logger.LogWarning("OSRM returned non-Ok code: {Code}", code);
                return (false, null, null, "Belirtilen duraklar arasında rota bulunamadı.");
            }

            var routeElement = json.RootElement.GetProperty("routes")[0];

            var coordinatesJson = routeElement
                .GetProperty("geometry")
                .GetProperty("coordinates");

            var lineCoordinates = coordinatesJson
                .EnumerateArray()
                .Select(pt => new Coordinate(pt[0].GetDouble(), pt[1].GetDouble()))
                .ToArray();

            var lineString      = GeometryFactory.CreateLineString(lineCoordinates);
            var durationSeconds = routeElement.GetProperty("duration").GetDouble();

            return (true, lineString, durationSeconds, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OSRM routing request failed");
            return (false, null, null, "Sunucu hatası.");
        }
    }
}