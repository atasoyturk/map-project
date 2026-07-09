using BackendApi.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace BackendApi.Controllers;

[Route("api/proxy/geoserver")]
public sealed class GeoServerProxyController : ApiControllerBase
{
    private readonly IHttpClientFactory          _httpClientFactory;
    private readonly GeoServerSettings           _settings;
    private readonly ILogger<GeoServerProxyController> _logger;

    public GeoServerProxyController(
        IHttpClientFactory               httpClientFactory,
        IOptions<GeoServerSettings>      settings,
        ILogger<GeoServerProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings          = settings.Value;
        _logger            = logger;
    }

    [HttpGet("wfs")]
    public async Task<IActionResult> GetWfs([FromQuery] string typeName)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(typeName))
            return BadRequest("typeName parametresi zorunludur.");

        // just allowed layers 
        var allowedLayers = new HashSet<string>
        {
            "tbl_point", "tbl_line", "tbl_polygon"
        };

        if (!allowedLayers.Contains(typeName))
            return BadRequest("Geçersiz katman adı.");

        try
        {
            var client = _httpClientFactory.CreateClient("GeoServer");

            // CQL_FILTER, user just sees self data 
            // like WHERE clause in sql
            var cqlFilter = $"\"UserId\"={userId.Value}";

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["service"]      = "WFS";
            query["version"]      = "2.0.0";
            query["request"]      = "GetFeature";
            query["typeNames"]    = $"{_settings.Workspace}:{typeName}";
            query["outputFormat"] = "application/json";
            query["CQL_FILTER"]   = cqlFilter;

            var url = $"{_settings.BaseUrl}/{_settings.Workspace}/wfs?{query}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GeoServer WFS request failed: {StatusCode}", response.StatusCode);
                return StatusCode(502, "GeoServer'dan veri alınamadı.");
            }

            var content     = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.ToString()
                              ?? "application/json";

            return Content(content, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GeoServer proxy failed for layer {TypeName}", typeName);
            return StatusCode(500, "Sunucu hatası.");
        }
    }
}