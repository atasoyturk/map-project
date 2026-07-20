using BackendApi.Authorization;
using BackendApi.DTOs.Admin;
using BackendApi.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Admin;

[Route("api/admin")]
[RequirePermission("admin_access")]
public sealed class AdminController : ApiControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService) => _adminService = adminService;

    //Users 

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers() => Ok(await _adminService.GetAllUsersAsync());

     [HttpPost("users")]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
    {
        var success = await _adminService.CreateEmployeeAsync(dto.Email, dto.Password);
        return success ? Created(string.Empty, null) : Conflict("Bu e-posta adresi zaten kayıtlı.");
    }

    [HttpPut("users/{id:int}/active")]
    public async Task<IActionResult> SetUserActive(int id, [FromBody] UpdateUserDto dto)
    {
        var result = await _adminService.SetUserActiveAsync(id, dto.IsActive);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("users/{id:int}/roles")]
    public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleDto dto)
    {
        if (dto.RoleId == 1)
            return StatusCode(403, "Admin rolü bu arayüzden atanamaz.");

        await _adminService.AssignRoleToUserAsync(id, dto.RoleId);
        return NoContent();
    }

    [HttpDelete("users/{id:int}/roles/{roleId:int}")]
    public async Task<IActionResult> RemoveRole(int id, int roleId)
    {
        var result = await _adminService.RemoveRoleFromUserAsync(id, roleId);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("users/{id:int}/permissions")]
    public async Task<IActionResult> AssignPermission(int id, [FromBody] AssignPermissionDto dto)
    {
        var currentUserId = GetUserId();
        if (currentUserId is null) return Unauthorized();

        if (dto.PermissionId == 4 && id == currentUserId.Value)
            return StatusCode(403, "Kendinize admin yetkisi veremezsiniz.");

        await _adminService.AssignPermissionToUserAsync(id, dto.PermissionId);
        return NoContent();
    }

    [HttpDelete("users/{id:int}/permissions/{permissionId:int}")]
    public async Task<IActionResult> RemovePermission(int id, int permissionId)
    {
        var result = await _adminService.RemovePermissionFromUserAsync(id, permissionId);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("users/{id:int}/permissions")]
    public async Task<IActionResult> GetUserPermissions(int id)
        => Ok(await _adminService.GetUserPermissionsAsync(id));

    // Roles

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles() => Ok(await _adminService.GetAllRolesAsync());

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
    {
        var role = await _adminService.CreateRoleAsync(dto.Name);
        return Created(string.Empty, role);
    }

    [HttpDelete("roles/{id:int}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        if (id is >= 1 and <= 4)
            return StatusCode(403, "Çekirdek roller (Admin, Çalışan, Stajyer, Takım Lideri) silinemez.");

        var result = await _adminService.DeleteRoleAsync(id);
        return result ? NoContent() : NotFound();
    }

    // Teams

    [HttpGet("teams")]
    public async Task<IActionResult> GetTeams() => Ok(await _adminService.GetAllTeamsAsync());

    [HttpPost("teams")]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
    {
        var team = await _adminService.CreateTeamAsync(dto.Name);
        return Created(string.Empty, team);
    }

    [HttpDelete("teams/{id:int}")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
        var result = await _adminService.DeleteTeamAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPut("users/{id:int}/team")]
    public async Task<IActionResult> AssignTeam(int id, [FromBody] AssignTeamDto dto)
    {
        var result = await _adminService.AssignTeamToUserAsync(id, dto.TeamId);
        return result ? NoContent() : NotFound();
    }

    [HttpPut("users/team/bulk")]
    public async Task<IActionResult> AssignTeamBulk([FromBody] AssignTeamBulkDto dto)
    {
        var updated = await _adminService.AssignTeamToUsersAsync(dto.UserIds, dto.TeamId);
        return Ok(new { updated });
    }
}