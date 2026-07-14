using BackendApi.Authorization;
using BackendApi.DTOs.Admin;
using BackendApi.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendApi.Controllers;

[Route("api/admin")]
[RequirePermission("admin_access")]
public sealed class AdminController : ApiControllerBase
{
    private readonly IAdminService            _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger       = logger;
    }

    //Users 

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        try { return Ok(await _adminService.GetAllUsersAsync()); }
        catch (Exception ex) { _logger.LogError(ex, "GetUsers failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpGet("teams")]
    public async Task<IActionResult> GetTeams()
    {
        try { return Ok(await _adminService.GetAllTeamsAsync()); }
        catch (Exception ex) { _logger.LogError(ex, "GetTeams failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPut("users/{id:int}/active")]
    public async Task<IActionResult> SetUserActive(int id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var result = await _adminService.SetUserActiveAsync(id, dto.IsActive);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "SetUserActive failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("users/{id:int}/roles")]
    public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleDto dto)
    {   
        if (dto.RoleId == 1)
            return StatusCode(403, "Admin rolü bu arayüzden atanamaz.");
        
        try
        {
            await _adminService.AssignRoleToUserAsync(id, dto.RoleId);
            return NoContent();
        }
        catch (Exception ex) { _logger.LogError(ex, "AssignRole failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPut("users/{id:int}/team")]
    public async Task<IActionResult> AssignTeam(int id, [FromBody] AssignTeamDto dto)
    {
        try
        {
            var result = await _adminService.AssignTeamToUserAsync(id, dto.TeamId);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "AssignTeam failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("teams")]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
    {
        try
        {
            var team = await _adminService.CreateTeamAsync(dto.Name);
            return Created(string.Empty, team);
        }
        catch (Exception ex) { _logger.LogError(ex, "CreateTeam failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("teams/{id:int}")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
        try
        {
            var result = await _adminService.DeleteTeamAsync(id);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "DeleteTeam failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("users/{id:int}/roles/{roleId:int}")]
    public async Task<IActionResult> RemoveRole(int id, int roleId)
    {
        try
        {
            var result = await _adminService.RemoveRoleFromUserAsync(id, roleId);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "RemoveRole failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("users/{id:int}/permissions")]
    public async Task<IActionResult> AssignPermission(int id, [FromBody] AssignPermissionDto dto)
    {
        // prevent self admin assign
        var currentUserId = GetUserId();
        if (currentUserId is null) return Unauthorized();

        if (dto.PermissionId == 4 && id == currentUserId.Value)
            return StatusCode(403, "Kendinize admin yetkisi veremezsiniz.");

        try
        {
            await _adminService.AssignPermissionToUserAsync(id, dto.PermissionId);
            return NoContent();
        }
        catch (Exception ex) { _logger.LogError(ex, "AssignPermission failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("users/{id:int}/permissions/{permissionId:int}")]
    public async Task<IActionResult> RemovePermission(int id, int permissionId)
    {
        try
        {
            var result = await _adminService.RemovePermissionFromUserAsync(id, permissionId);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "RemovePermission failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpGet("users/{id:int}/permissions")]
    public async Task<IActionResult> GetUserPermissions(int id)
    {
        try { return Ok(await _adminService.GetUserPermissionsAsync(id)); }
        catch (Exception ex) { _logger.LogError(ex, "GetUserPermissions failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    // Roles

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        try { return Ok(await _adminService.GetAllRolesAsync()); }
        catch (Exception ex) { _logger.LogError(ex, "GetRoles failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
    {
        try
        {
            var role = await _adminService.CreateRoleAsync(dto.Name);
            return Created(string.Empty, role);
        }
        catch (Exception ex) { _logger.LogError(ex, "CreateRole failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("roles/{id:int}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        if (id is >= 1 and <= 4)
            return StatusCode(403, "Çekirdek roller (Admin, Çalışan, Stajyer, Takım Lideri) silinemez.");

        try
        {
            var result = await _adminService.DeleteRoleAsync(id);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "DeleteRole failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }
}