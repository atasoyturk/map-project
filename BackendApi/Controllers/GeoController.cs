using BackendApi.DTOs;
using BackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendApi.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public sealed class GeoController : ControllerBase
{
    private readonly IPointService          _pointService;
    private readonly ILineService           _lineService;
    private readonly IPolygonService        _polygonService;
    private readonly ILogger<GeoController> _logger;

    public GeoController(
        IPointService          pointService,
        ILineService           lineService,
        IPolygonService        polygonService,
        ILogger<GeoController> logger)
    {
        _pointService   = pointService;
        _lineService    = lineService;
        _polygonService = polygonService;
        _logger         = logger;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("point")]
    public async Task<IActionResult> GetPoints()
    {
        try { return Ok(await _pointService.GetAllAsync(GetUserId())); }
        catch (Exception ex) { _logger.LogError(ex, "GetPoints failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("point")]
    public async Task<IActionResult> SavePoint([FromBody] GeoRequestDto request)
    {
        try { return Created(string.Empty, await _pointService.SaveAsync(request, GetUserId())); }
        catch (Exception ex) { _logger.LogError(ex, "SavePoint failed"); return BadRequest("Geçersiz WKT formatı."); }
    }

    [HttpPut("point/{id:int}")]
    public async Task<IActionResult> UpdatePoint(int id, [FromBody] GeoRequestDto request)
    {
        try
        {
            var result = await _pointService.UpdateAsync(id, request, GetUserId());
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex) { _logger.LogError(ex, "UpdatePoint failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpGet("line")]
    public async Task<IActionResult> GetLines()
    {
        try { return Ok(await _lineService.GetAllAsync(GetUserId())); }
        catch (Exception ex) { _logger.LogError(ex, "GetLines failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("line")]
    public async Task<IActionResult> SaveLine([FromBody] GeoRequestDto request)
    {
        try { return Created(string.Empty, await _lineService.SaveAsync(request, GetUserId())); }
        catch (Exception ex) { _logger.LogError(ex, "SaveLine failed"); return BadRequest("Geçersiz WKT formatı."); }
    }

    [HttpPut("line/{id:int}")]
    public async Task<IActionResult> UpdateLine(int id, [FromBody] GeoRequestDto request)
    {
        try
        {
            var result = await _lineService.UpdateAsync(id, request, GetUserId());
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex) { _logger.LogError(ex, "UpdateLine failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpGet("polygon")]
    public async Task<IActionResult> GetPolygons()
    {
        try { return Ok(await _polygonService.GetAllAsync(GetUserId())); }
        catch (Exception ex) { _logger.LogError(ex, "GetPolygons failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("polygon")]
    public async Task<IActionResult> SavePolygon([FromBody] GeoRequestDto request)
    {
        try { return Created(string.Empty, await _polygonService.SaveAsync(request, GetUserId())); }
        catch (Exception ex) { _logger.LogError(ex, "SavePolygon failed"); return BadRequest("Geçersiz WKT formatı."); }
    }

    [HttpPut("polygon/{id:int}")]
    public async Task<IActionResult> UpdatePolygon(int id, [FromBody] GeoRequestDto request)
    {
        try
        {
            var result = await _polygonService.UpdateAsync(id, request, GetUserId());
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex) { _logger.LogError(ex, "UpdatePolygon failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }
}