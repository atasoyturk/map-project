using BackendApi.DTOs;
using BackendApi.Services.Geo;
using Microsoft.AspNetCore.Mvc;


namespace BackendApi.Controllers;

[Route("api")]
public sealed class GeoController : ApiControllerBase
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

    //  Point 

    [HttpGet("point")]
    public async Task<IActionResult> GetPoints()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try { return Ok(await _pointService.GetAllAsync(userId.Value)); }
        catch (Exception ex) { _logger.LogError(ex, "GetPoints failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpGet("point/{id:int}")]
    public async Task<IActionResult> GetPoint(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var result = await _pointService.GetByIdAsync(id, userId.Value);
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex) { _logger.LogError(ex, "GetPoint failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("point")]
    public async Task<IActionResult> SavePoint([FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var roles = GetUserRoles();
            return Created(string.Empty, await _pointService.SaveAsync(request, userId.Value, roles));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SavePoint failed");
            return BadRequest("Geçersiz WKT formatı.");
        }
    }

    [HttpPut("point/{id:int}")]
    public async Task<IActionResult> UpdatePoint(int id, [FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var roles  = GetUserRoles();
            var result = await _pointService.UpdateAsync(id, request, userId.Value, roles);
            return result is null ? NotFound() : Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex) { _logger.LogError(ex, "UpdatePoint failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("point/{id:int}")]
    public async Task<IActionResult> DeletePoint(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var result = await _pointService.DeleteAsync(id, userId.Value);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "DeletePoint failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    //  Line 

    [HttpGet("line")]
    public async Task<IActionResult> GetLines()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try { return Ok(await _lineService.GetAllAsync(userId.Value)); }
        catch (Exception ex) { _logger.LogError(ex, "GetLines failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpGet("line/{id:int}")]
    public async Task<IActionResult> GetLine(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var result = await _lineService.GetByIdAsync(id, userId.Value);
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex) { _logger.LogError(ex, "GetLine failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("line")]
    public async Task<IActionResult> SaveLine([FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var roles = GetUserRoles();
            return Created(string.Empty, await _lineService.SaveAsync(request, userId.Value, roles));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveLine failed");
            return BadRequest("Geçersiz WKT formatı.");
        }
    }

    [HttpPut("line/{id:int}")]
    public async Task<IActionResult> UpdateLine(int id, [FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var roles  = GetUserRoles();
            var result = await _lineService.UpdateAsync(id, request, userId.Value, roles);
            return result is null ? NotFound() : Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex) { _logger.LogError(ex, "UpdateLine failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("line/{id:int}")]
    public async Task<IActionResult> DeleteLine(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var result = await _lineService.DeleteAsync(id, userId.Value);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "DeleteLine failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    //  Polygon 

    [HttpGet("polygon")]
    public async Task<IActionResult> GetPolygons()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try { return Ok(await _polygonService.GetAllAsync(userId.Value)); }
        catch (Exception ex) { _logger.LogError(ex, "GetPolygons failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpGet("polygon/{id:int}")]
    public async Task<IActionResult> GetPolygon(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var result = await _polygonService.GetByIdAsync(id, userId.Value);
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex) { _logger.LogError(ex, "GetPolygon failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("polygon")]
    public async Task<IActionResult> SavePolygon([FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var roles = GetUserRoles();
            return Created(string.Empty, await _polygonService.SaveAsync(request, userId.Value, roles));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SavePolygon failed");
            return BadRequest("Geçersiz WKT formatı.");
        }
    }

    [HttpPut("polygon/{id:int}")]
    public async Task<IActionResult> UpdatePolygon(int id, [FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var roles  = GetUserRoles();
            var result = await _polygonService.UpdateAsync(id, request, userId.Value, roles);
            return result is null ? NotFound() : Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex) { _logger.LogError(ex, "UpdatePolygon failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("polygon/{id:int}")]
    public async Task<IActionResult> DeletePolygon(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var result = await _polygonService.DeleteAsync(id, userId.Value);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "DeletePolygon failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }
}