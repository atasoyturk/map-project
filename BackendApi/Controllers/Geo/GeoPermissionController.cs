using BackendApi.Authorization;
using BackendApi.DTOs.Geo;
using BackendApi.Services.Geo;
using Microsoft.AspNetCore.Mvc;


namespace BackendApi.Controllers.Geo;

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

    //  GeoPermission CRUD 

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
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Sınır adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(request.WktGeometry))
            return BadRequest("Geometri boş olamaz.");

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

    //  User assignment 

    [HttpGet("user/{userId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        try { return Ok(await _geoPermissionService.GetByUserAsync(userId)); }
        catch (Exception ex) { _logger.LogError(ex, "GetByUser GeoPermission failed for userId {UserId}", userId); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("user/{userId:int}/{geoPermissionId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> AssignToUser(int userId, int geoPermissionId)
    {
        try
        {
            await _geoPermissionService.AssignToUserAsync(userId, geoPermissionId);
            return NoContent();
        }
        catch (Exception ex) { _logger.LogError(ex, "AssignToUser GeoPermission failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("user/{userId:int}/{geoPermissionId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> RemoveFromUser(int userId, int geoPermissionId)
    {
        try
        {
            var result = await _geoPermissionService.RemoveFromUserAsync(userId, geoPermissionId);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "RemoveFromUser GeoPermission failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    //  Role assignment 

    [HttpGet("role/{roleId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> GetByRole(int roleId)
    {
        try { return Ok(await _geoPermissionService.GetByRoleAsync(roleId)); }
        catch (Exception ex) { _logger.LogError(ex, "GetByRole GeoPermission failed for roleId {RoleId}", roleId); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("role/{roleId:int}/{geoPermissionId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> AssignToRole(int roleId, int geoPermissionId)
    {
        try
        {
            await _geoPermissionService.AssignToRoleAsync(roleId, geoPermissionId);
            return NoContent();
        }
        catch (Exception ex) { _logger.LogError(ex, "AssignToRole GeoPermission failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("role/{roleId:int}/{geoPermissionId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> RemoveFromRole(int roleId, int geoPermissionId)
    {
        try
        {
            var result = await _geoPermissionService.RemoveFromRoleAsync(roleId, geoPermissionId);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "RemoveFromRole GeoPermission failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] GeoValidateRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var roles  = GetUserRoles();
            var result = await _geoPermissionService.ValidateGeometryAsync(
                userId.Value, roles, request.WktGeometry);

            return result ? Ok() : StatusCode(403, "Bu alan dışında çizim yapma yetkiniz bulunmamaktadır.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validate GeoPermission failed");
            return StatusCode(500, "Sunucu hatası.");
        }
    }

    [HttpPut("{id:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Update(int id, [FromBody] GeoPermissionRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Sınır adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(request.WktGeometry))
            return BadRequest("Geometri boş olamaz.");

        try
        {
            var result = await _geoPermissionService.UpdateAsync(id, request);
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update GeoPermission failed for id {Id}", id);
            return StatusCode(500, "Sunucu hatası.");
        }
    }
}