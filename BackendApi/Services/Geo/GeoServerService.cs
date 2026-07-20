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
        "tbl_point", "tbl_line", "tbl_polygon", "tbl_poi_active", "tbl_transit_stop_active"
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

    private static string BuildCqlFilter(string typeName, int userId, int? teamId, GeoViewMode viewMode)
    {
        if (typeName == "tbl_poi_active" || typeName == "tbl_transit_stop_active")
            return string.Empty;

        return viewMode switch
        {
            GeoViewMode.All  => "\"IsDeleted\"=false",
            GeoViewMode.Team => $"\"TeamId\"={teamId} AND \"IsDeleted\"=false",
            _                => $"\"UserId\"={userId} AND \"IsDeleted\"=false"
        };
    }

    public async Task<(bool Success, string? Content, string? ContentType, string? Error)>
        GetFeaturesAsync(string typeName, int userId, int? teamId, GeoViewMode viewMode)
        {
            if (!AllowedLayers.Contains(typeName))
                return (false, null, null, "Gecersiz katman adi.");

            try
            {
                var client    = _httpClientFactory.CreateClient("GeoServer");
                var cqlFilter = BuildCqlFilter(typeName, userId, teamId, viewMode);

                var query = HttpUtility.ParseQueryString(string.Empty);
                query["service"]      = "WFS";
                query["version"]      = "2.0.0";
                query["request"]      = "GetFeature";
                query["typeNames"]    = $"{_settings.Workspace}:{typeName}";
                query["outputFormat"] = "application/json";

                if (!string.IsNullOrEmpty(cqlFilter))
                    query["CQL_FILTER"] = cqlFilter;

                var url      = $"{_settings.BaseUrl}/{_settings.Workspace}/wfs?{query}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("GeoServer WFS failed: {StatusCode}", response.StatusCode);
                    return (false, null, null, "GeoServer'dan veri alinamadi.");
                }

                var content     = await response.Content.ReadAsStringAsync();
                var contentType = response.Content.Headers.ContentType?.ToString()
                                ?? "application/json";

                return (true, content, contentType, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GeoServer proxy failed for {TypeName}", typeName);
                return (false, null, null, "Sunucu hatasi.");
            }
        }

    public async Task<(bool Success, byte[]? Content, string? ContentType, string? Error)>
        GetWmsMapAsync(string typeName, int userId, string bbox, int width, int height, string? styles, int? teamId, GeoViewMode viewMode)
        {
            if (!AllowedLayers.Contains(typeName))
                return (false, null, null, "Gecersiz katman adi.");

            try
            {
                var client    = _httpClientFactory.CreateClient("GeoServer");
                var cqlFilter = BuildCqlFilter(typeName, userId, teamId, viewMode);

                var query = HttpUtility.ParseQueryString(string.Empty);
                query["service"]     = "WMS";
                query["version"]     = "1.1.1";
                query["request"]     = "GetMap";
                query["layers"]      = $"{_settings.Workspace}:{typeName}";
                query["bbox"]        = bbox;
                query["width"]       = width.ToString();
                query["height"]      = height.ToString();
                query["srs"]         = "EPSG:3857";
                query["format"]      = "image/png";
                query["transparent"] = "true";

                if (!string.IsNullOrEmpty(cqlFilter))
                    query["CQL_FILTER"] = cqlFilter;

                if (!string.IsNullOrWhiteSpace(styles))
                    query["styles"] = styles;

                var url      = $"{_settings.BaseUrl}/{_settings.Workspace}/wms?{query}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("GeoServer WMS failed: {StatusCode}", response.StatusCode);
                    return (false, null, null, "GeoServer'dan goruntu alinamadi.");
                }

                var content     = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString()
                                ?? "image/png";

                return (true, content, contentType, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GeoServer WMS proxy failed for {TypeName}", typeName);
                return (false, null, null, "Sunucu hatasi.");
            }
        }
}
