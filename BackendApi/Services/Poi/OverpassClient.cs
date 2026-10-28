using System.Text.Json;
using BackendApi.Settings;
using Microsoft.Extensions.Options;

namespace BackendApi.Services.Poi;

public sealed class OverpassClient : IOverpassClient
{
    private readonly IHttpClientFactory      _httpClientFactory;
    private readonly OverpassSettings        _settings;
    private readonly ILogger<OverpassClient> _logger;

    public OverpassClient(
        IHttpClientFactory         httpClientFactory,
        IOptions<OverpassSettings> settings,
        ILogger<OverpassClient>    logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings          = settings.Value;
        _logger            = logger;
    }

    public async Task<IReadOnlyList<OsmElement>> RunQueryAsync(string query, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("Overpass");

        using var response = await client.PostAsync(
            _settings.BaseUrl,
            new FormUrlEncodedContent([new KeyValuePair<string, string>("data", query)]),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Overpass request failed: {StatusCode}", response.StatusCode);
            throw new HttpRequestException($"Overpass request failed: {response.StatusCode}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        // Overpass reports timeouts / maxsize overruns as HTTP 200 with a "remark" and partial data.
        if (json.RootElement.TryGetProperty("remark", out var remark))
        {
            _logger.LogError("Overpass returned a remark: {Remark}", remark.GetString());
            throw new HttpRequestException($"Overpass query incomplete: {remark.GetString()}");
        }

        if (!json.RootElement.TryGetProperty("elements", out var elementsJson))
            throw new HttpRequestException("Overpass response has no 'elements'.");

        var elements = new List<OsmElement>();
        foreach (var e in elementsJson.EnumerateArray())
        {
            var position = e.TryGetProperty("center", out var center) ? center : e;
            if (!position.TryGetProperty("lat", out var lat) || !position.TryGetProperty("lon", out var lon)
                || !e.TryGetProperty("type", out var type) || !e.TryGetProperty("id", out var id))
                continue;

            var tags = new Dictionary<string, string>();
            if (e.TryGetProperty("tags", out var tagsElement))
                foreach (var tag in tagsElement.EnumerateObject())
                    tags[tag.Name] = tag.Value.GetString() ?? string.Empty;

            elements.Add(new OsmElement(
                type.GetString()!,
                id.GetInt64(),
                lat.GetDouble(),
                lon.GetDouble(),
                tags));
        }

        return elements;
    }
}
