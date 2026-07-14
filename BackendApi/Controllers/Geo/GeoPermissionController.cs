using BackendApi.Authorization;
using BackendApi.DTOs.Geo;
using BackendApi.Services.Geo;
using Microsoft.AspNetCore.Mvc;


namespace BackendApi.Controllers.Geo;

[Route("api/geo-permission")]
public sealed class GeoPermissionController : ApiControllerBase
{
    private readonly IGeoPermissionService _geoPermissionService;

    public GeoPermissionController(IGeoPermissionService geoPermissionService)
        => _geoPermissionService = geoPermissionService;

    //  GeoPermission CRUD 

    [HttpGet]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> GetAll() => Ok(await _geoPermissionService.GetAllAsync());

    [HttpPost]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Create([FromBody] GeoPermissionRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Sınır adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(request.WktGeometry))
            return BadRequest("Geometri boş olamaz.");

        var result = await _geoPermissionService.CreateAsync(request);
        return Created(string.Empty, result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _geoPermissionService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }

    //  User assignment 

    [HttpGet("user/{userId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> GetByUser(int userId)
        => Ok(await _geoPermissionService.GetByUserAsync(userId));

    [HttpPost("user/{userId:int}/{geoPermissionId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> AssignToUser(int userId, int geoPermissionId)
    {
        await _geoPermissionService.AssignToUserAsync(userId, geoPermissionId);
        return NoContent();
    }

    [HttpDelete("user/{userId:int}/{geoPermissionId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> RemoveFromUser(int userId, int geoPermissionId)
    {
        var result = await _geoPermissionService.RemoveFromUserAsync(userId, geoPermissionId);
        return result ? NoContent() : NotFound();
    }

    //  Role assignment 

    [HttpGet("role/{roleId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> GetByRole(int roleId)
        => Ok(await _geoPermissionService.GetByRoleAsync(roleId));

    [HttpPost("role/{roleId:int}/{geoPermissionId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> AssignToRole(int roleId, int geoPermissionId)
    {
        await _geoPermissionService.AssignToRoleAsync(roleId, geoPermissionId);
        return NoContent();
    }

    [HttpDelete("role/{roleId:int}/{geoPermissionId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> RemoveFromRole(int roleId, int geoPermissionId)
    {
        var result = await _geoPermissionService.RemoveFromRoleAsync(roleId, geoPermissionId);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] GeoValidateRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var roles  = GetUserRoles();
        var result = await _geoPermissionService.ValidateGeometryAsync(
            userId.Value, roles, request.WktGeometry);

        return result ? Ok() : StatusCode(403, "Bu alan dışında çizim yapma yetkiniz bulunmamaktadır.");
    }

    [HttpPut("{id:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Update(int id, [FromBody] GeoPermissionRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Sınır adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(request.WktGeometry))
            return BadRequest("Geometri boş olamaz.");

        var result = await _geoPermissionService.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }
}