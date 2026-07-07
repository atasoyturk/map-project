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
}