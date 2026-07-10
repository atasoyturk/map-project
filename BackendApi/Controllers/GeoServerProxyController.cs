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
    public async Task<IActionResult> GetWfs([FromQuery] string typeName)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(typeName))
            return BadRequest("typeName parametresi zorunludur.");

        var (success, content, contentType, error) =
            await _geoServerService.GetFeaturesAsync(typeName, userId.Value);

        if (!success)
            return error == "Geçersiz katman adı."
                ? BadRequest(error)
                : StatusCode(502, error);

        return Content(content!, contentType!);
    }
}