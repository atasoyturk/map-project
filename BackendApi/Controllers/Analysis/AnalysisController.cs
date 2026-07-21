using BackendApi.DTOs.Geo;
using BackendApi.Services.Analysis;
using BackendApi.DTOs.Analysis;
using BackendApi.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Analysis;

[Route("api/analysis")]
public sealed class AnalysisController : ApiControllerBase
{
    private readonly IAnalysisService _analysisService;

    public AnalysisController(IAnalysisService analysisService)
        => _analysisService = analysisService;

    [HttpPost("temp-inventory")]
    [RequirePermission("area_scan")]
    public async Task<IActionResult> TempInventory([FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var count = await _analysisService.TempInventoryAsync(request, userId.Value);
        return Ok(new { intersectedInventoryCount = count });
    }

    [HttpPost("location-analysis")]
    [RequirePermission("location_analysis")]
    public async Task<IActionResult> LocationAnalysis([FromBody] LocationAnalysisRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var result = await _analysisService.LocationAnalysisAsync(request, userId.Value);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}