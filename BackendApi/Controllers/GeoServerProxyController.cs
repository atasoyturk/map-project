using BackendApi.Services.Geo;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[Route("api/proxy/geoserver")]
public sealed class GeoServerProxyController : ApiControllerBase
{
    private readonly IGeoServerService            _geoServerService;
    private readonly ILogger<GeoServerProxyController> _logger;

    public GeoServerProxyController(
        IGeoServerService                geoServerService,
        ILogger<GeoServerProxyController> logger)
    {
        _geoServerService = geoServerService;
        _logger           = logger;
    }

    [HttpGet("wfs")]
    public async Task<IActionResult> GetWfs(
        [FromQuery] string  typeName,
        [FromQuery] string? viewMode = null)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(typeName))
            return BadRequest("typeName parametresi zorunludur.");

        var teamId       = GetTeamId();
        var resolvedMode = GeoViewModeResolver.Resolve(GetUserRoles(), teamId, viewMode);

        var (success, content, contentType, error) =
            await _geoServerService.GetFeaturesAsync(typeName, userId.Value, teamId, resolvedMode);

        if (!success)
            return error == "Geçersiz katman adı."
                ? BadRequest(error)
                : StatusCode(502, error);

        return Content(content!, contentType!);
    }

    [HttpGet("wms")]
    public async Task<IActionResult> GetWms(
        [FromQuery] string  typeName,
        [FromQuery] string  bbox,
        [FromQuery] int     width,
        [FromQuery] int     height,
        [FromQuery] string? styles  = null,
        [FromQuery] string? viewMode = null)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(typeName))
            return BadRequest("typeName parametresi zorunludur.");

        if (string.IsNullOrWhiteSpace(bbox))
            return BadRequest("bbox parametresi zorunludur.");

        if (width <= 0 || height <= 0)
            return BadRequest("Geçersiz boyut.");

        var teamId       = GetTeamId();
        var resolvedMode = GeoViewModeResolver.Resolve(GetUserRoles(), teamId, viewMode);

        var (success, content, contentType, error) =
            await _geoServerService.GetWmsMapAsync(typeName, userId.Value, bbox, width, height, styles, teamId, resolvedMode);

        if (!success)
            return error == "Geçersiz katman adı."
                ? BadRequest(error)
                : StatusCode(502, error);

        return File(content!, contentType!);
    }
}