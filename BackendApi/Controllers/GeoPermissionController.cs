using BackendApi.Authorization;
using BackendApi.DTOs.Geo;
using BackendApi.Services.Geo;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[Route("api/geo-permission")]
public sealed class GeoPermissionController : ApiControllerBase
{
    private readonly IGeoPermissionService            _geoPermissionService;
    private readonly ILogger<GeoPermissionController> _logger;

    public GeoPermissionController(
        IGeoPermissionService            geoPermissionService,
        ILogger<GeoPermissionController> logger)
    {
        _geoPermissionService = geoPermissionService;
        _logger               = logger;
    }

    [HttpGet]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> GetAll()
    {
        try { return Ok(await _geoPermissionService.GetAllAsync()); }
        catch (Exception ex) { _logger.LogError(ex, "GetAll GeoPermission failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Create([FromBody] GeoPermissionRequestDto request)
    {
        if (request.UserId is null && request.RoleId is null)
            return BadRequest("UserId veya RoleId belirtilmelidir.");

        try
        {
            var result = await _geoPermissionService.CreateAsync(request);
            return Created(string.Empty, result);
        }
        catch (Exception ex) { _logger.LogError(ex, "Create GeoPermission failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _geoPermissionService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "Delete GeoPermission failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }
}