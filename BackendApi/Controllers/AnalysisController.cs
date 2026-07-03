using BackendApi.DTOs;
using BackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/analysis")]
[Authorize]
public sealed class AnalysisController : ControllerBase
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

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("temp-inventory")]
    public async Task<IActionResult> TempInventory([FromBody] GeoRequestDto request)
    {
        try
        {
            var count = await _analysisService.TempInventoryAsync(request, GetUserId());
            return Ok(new { intersectedInventoryCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TempInventory failed");
            return StatusCode(500, "Sunucu hatası.");
        }
    }
}