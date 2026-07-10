using BackendApi.Settings;
using Microsoft.Extensions.Options;
using System.Web;

namespace BackendApi.Services.Geo;

public sealed class GeoServerService : IGeoServerService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GeoServerSettings  _settings;
    private readonly ILogger<GeoServerService> _logger;

    private static readonly HashSet<string> AllowedLayers = new()
    {
        "tbl_point", "tbl_line", "tbl_polygon"
    };

    public GeoServerService(
        IHttpClientFactory         httpClientFactory,
        IOptions<GeoServerSettings> settings,
        ILogger<GeoServerService>  logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings          = settings.Value;
        _logger            = logger;
    }

    public async Task<(bool Success, string? Content, string? ContentType, string? Error)>
        GetFeaturesAsync(string typeName, int userId)
    {
        if (!AllowedLayers.Contains(typeName))
            return (false, null, null, "Geçersiz katman adı.");

        try
        {
            var client    = _httpClientFactory.CreateClient("GeoServer");
            var cqlFilter = $"\"UserId\"={userId} AND \"IsDeleted\"=false";

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["service"]      = "WFS";
            query["version"]      = "2.0.0";
            query["request"]      = "GetFeature";
            query["typeNames"]    = $"{_settings.Workspace}:{typeName}";
            query["outputFormat"] = "application/json";
            query["CQL_FILTER"]   = cqlFilter;

            var url      = $"{_settings.BaseUrl}/{_settings.Workspace}/wfs?{query}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GeoServer WFS failed: {StatusCode}", response.StatusCode);
                return (false, null, null, "GeoServer'dan veri alınamadı.");
            }

            var content     = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.ToString()
                              ?? "application/json";

            return (true, content, contentType, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GeoServer proxy failed for {TypeName}", typeName);
            return (false, null, null, "Sunucu hatası.");
        }
    }
}