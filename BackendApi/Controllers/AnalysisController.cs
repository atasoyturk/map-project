using BackendApi.DTOs.Geo;
using BackendApi.Services.Analysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendApi.Controllers;

[Route("api/analysis")]
public sealed class AnalysisController : ApiControllerBase
{
    private readonly IAnalysisService            _analysisService;
    private readonly ILogger<AnalysisController> _logger;

    public AnalysisController(
        IAnalysisService            analysisService,
        ILogger<AnalysisController> logger)
    {
        _analysisService = analysisService;
        _logger          = logger;
    }


    [HttpPost("temp-inventory")]
    public async Task<IActionResult> TempInventory([FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var count = await _analysisService.TempInventoryAsync(request, userId.Value);
            return Ok(new { intersectedInventoryCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TempInventory failed");
            return StatusCode(500, "Sunucu hatası.");
        }
    }
}