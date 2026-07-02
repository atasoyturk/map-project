using BackendApi.DTOs;
using BackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

// takes DI -> Call services -> return HTTP response

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
        try
        {
            var result = await _pointService.SaveAsync(request);
            return Created(string.Empty, result);
        }
        catch (Exception)
        {
            return BadRequest("Geçersiz WKT formatı.");
        }
    }

    [HttpPost("line")]
    public async Task<IActionResult> SaveLine([FromBody] GeoRequestDto request)
    {
        try
        {
            var result = await _lineService.SaveAsync(request);
            return Created(string.Empty, result);
        }
        catch (Exception)
        {
            return BadRequest("Geçersiz WKT formatı.");
        }
    }

    [HttpPost("polygon")]
    public async Task<IActionResult> SavePolygon([FromBody] GeoRequestDto request)
    {
        try
        {
            var result = await _polygonService.SaveAsync(request);
            return Created(string.Empty, result);
        }
        catch (Exception)
        {
            return BadRequest("Geçersiz WKT formatı.");
        }
    }
}