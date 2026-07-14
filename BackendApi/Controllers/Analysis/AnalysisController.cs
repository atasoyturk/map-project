using BackendApi.DTOs.Geo;
using BackendApi.Services.Analysis;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Analysis;

[Route("api/analysis")]
public sealed class AnalysisController : ApiControllerBase
{
    private readonly IAnalysisService _analysisService;

    public AnalysisController(IAnalysisService analysisService)
        => _analysisService = analysisService;

    [HttpPost("temp-inventory")]
    public async Task<IActionResult> TempInventory([FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var count = await _analysisService.TempInventoryAsync(request, userId.Value);
        return Ok(new { intersectedInventoryCount = count });
    }
}