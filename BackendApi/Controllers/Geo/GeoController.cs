using BackendApi.DTOs.Geo;
using BackendApi.DTOs.Admin;
using BackendApi.Services.Geo;
using Microsoft.AspNetCore.Mvc;


namespace BackendApi.Controllers.Geo;

[Route("api")]
public sealed class GeoController : ApiControllerBase
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

    //  Point 

    [HttpGet("point")]
    public async Task<IActionResult> GetPoints([FromQuery] string? viewMode = null)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var teamId       = GetTeamId();
        var resolvedMode = GeoViewModeResolver.Resolve(HasAdminAccess(), teamId, viewMode);
        return Ok(await _pointService.GetAllAsync(userId.Value, teamId, resolvedMode));
    }

    [HttpGet("point/{id:int}")]
    public async Task<IActionResult> GetPoint(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _pointService.GetByIdAsync(id, userId.Value);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("point")]
    public async Task<IActionResult> SavePoint([FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _pointService.SaveAsync(request, userId.Value, GetTeamId(), GetUserRoles());
        return Created(string.Empty, result);
    }

    [HttpPut("point/{id:int}")]
    public async Task<IActionResult> UpdatePoint(int id, [FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _pointService.UpdateAsync(id, request, userId.Value, GetUserRoles());
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("point/{id:int}")]
    public async Task<IActionResult> DeletePoint(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _pointService.DeleteAsync(id, userId.Value);
        return result ? NoContent() : NotFound();
    }

    //  Line 

    [HttpGet("line")]
    public async Task<IActionResult> GetLines([FromQuery] string? viewMode = null)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var teamId       = GetTeamId();
        var resolvedMode = GeoViewModeResolver.Resolve(HasAdminAccess(), teamId, viewMode);
        return Ok(await _lineService.GetAllAsync(userId.Value, teamId, resolvedMode));
    }

    [HttpGet("line/{id:int}")]
    public async Task<IActionResult> GetLine(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _lineService.GetByIdAsync(id, userId.Value);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("line")]
    public async Task<IActionResult> SaveLine([FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _lineService.SaveAsync(request, userId.Value, GetTeamId(), GetUserRoles());
        return Created(string.Empty, result);
    }

    [HttpPut("line/{id:int}")]
    public async Task<IActionResult> UpdateLine(int id, [FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _lineService.UpdateAsync(id, request, userId.Value, GetUserRoles());
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("line/{id:int}")]
    public async Task<IActionResult> DeleteLine(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _lineService.DeleteAsync(id, userId.Value);
        return result ? NoContent() : NotFound();
    }

    //  Polygon 

    [HttpGet("polygon")]
    public async Task<IActionResult> GetPolygons([FromQuery] string? viewMode = null)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var teamId       = GetTeamId();
        var resolvedMode = GeoViewModeResolver.Resolve(HasAdminAccess(), teamId, viewMode);
        return Ok(await _polygonService.GetAllAsync(userId.Value, teamId, resolvedMode));
    }

    [HttpGet("polygon/{id:int}")]
    public async Task<IActionResult> GetPolygon(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _polygonService.GetByIdAsync(id, userId.Value);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("polygon")]
    public async Task<IActionResult> SavePolygon([FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _polygonService.SaveAsync(request, userId.Value, GetTeamId(), GetUserRoles());
        return Created(string.Empty, result);
    }

    [HttpPut("polygon/{id:int}")]
    public async Task<IActionResult> UpdatePolygon(int id, [FromBody] GeoRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _polygonService.UpdateAsync(id, request, userId.Value, GetUserRoles());
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("polygon/{id:int}")]
    public async Task<IActionResult> DeletePolygon(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _polygonService.DeleteAsync(id, userId.Value);
        return result ? NoContent() : NotFound();
    }
}