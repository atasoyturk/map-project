using BackendApi.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Auth;

[Route("api/users")]
public sealed class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
        => _userService = userService;

    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup()
        => Ok(await _userService.GetLookupAsync());
}