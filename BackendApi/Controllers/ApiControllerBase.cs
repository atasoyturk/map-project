using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendApi.Controllers;

[ApiController]
[Authorize]
public abstract class ApiControllerBase : ControllerBase
{
    protected int? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : null;
    }

    protected int? GetTeamId()
    {
        var value = User.FindFirstValue("team_id");
        return int.TryParse(value, out var teamId) ? teamId : null;
    }

    protected bool HasAdminAccess() =>
        User.FindFirstValue("admin_access") == "true";

    protected IEnumerable<string> GetUserRoles() =>
    User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
}