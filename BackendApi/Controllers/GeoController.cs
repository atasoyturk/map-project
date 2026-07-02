using BackendApi.DTOs;
using BackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public sealed class GeoController : ControllerBase
{
    private readonly IPointService   _pointService;
    private readonly ILineService    _lineService;
    private readonly IPolygonService _polygonService;

    public GeoController(
        IPointService   pointService,
        ILineService    lineService,
        IPolygonService polygonService)
    {
        _pointService   = pointService;
        _lineService    = lineService;
        _polygonService = polygonService;
    }

    [HttpPost("point")]
    public async Task<IActionResult> SavePoint([FromBody] GeoRequestDto request)
    {
        try { return Created(string.Empty, await _pointService.SaveAsync(request)); }
        catch (Exception) { return BadRequest("Geçersiz WKT formatı."); }
    }

    [HttpPost("line")]
    public async Task<IActionResult> SaveLine([FromBody] GeoRequestDto request)
    {
        try { return Created(string.Empty, await _lineService.SaveAsync(request)); }
        catch (Exception) { return BadRequest("Geçersiz WKT formatı."); }
    }

    [HttpPost("polygon")]
    public async Task<IActionResult> SavePolygon([FromBody] GeoRequestDto request)
    {
        try { return Created(string.Empty, await _polygonService.SaveAsync(request)); }
        catch (Exception) { return BadRequest("Geçersiz WKT formatı."); }
    }

    [HttpGet("point")]
    public async Task<IActionResult> GetPoints() =>
        Ok(await _pointService.GetAllAsync());

    [HttpGet("line")]
    public async Task<IActionResult> GetLines() =>
        Ok(await _lineService.GetAllAsync());

    [HttpGet("polygon")]
    public async Task<IActionResult> GetPolygons() =>
        Ok(await _polygonService.GetAllAsync());
}