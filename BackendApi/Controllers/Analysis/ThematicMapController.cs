using BackendApi.Authorization;
using BackendApi.DTOs.Analysis;
using BackendApi.Services.Analysis;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Analysis;

[Route("api/thematic-map")]
public sealed class ThematicMapController : ApiControllerBase
{
    private readonly IThematicMapService _service;

    public ThematicMapController(IThematicMapService service) => _service = service;

    [HttpPost("isochrone")]
    [RequirePermission("transit_stop_read")]
    public async Task<IActionResult> GetIsochrone([FromBody] IsochroneRequestDto request)
    {
        var result = await _service.GetIsochroneAsync(request.CenterLon, request.CenterLat, request.MinutesThresholds);
        return Ok(result);
    }
}