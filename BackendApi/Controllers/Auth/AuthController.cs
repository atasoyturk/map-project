using Microsoft.AspNetCore.Mvc;
using BackendApi.DTOs.Auth;
using BackendApi.Repositories;
using BackendApi.Services.Auth;
using BackendApi.Services.Permission;

namespace BackendApi.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserRepository    _userRepository;
    private readonly IUserService       _userService;
    private readonly ITokenService      _tokenService;
    private readonly IPermissionService _permissionService;

    public AuthController(
        IUserRepository    userRepository,
        IUserService       userService,
        ITokenService      tokenService,
        IPermissionService permissionService)
    {
        _userRepository     = userRepository;
        _userService        = userService;
        _tokenService       = tokenService;
        _permissionService  = permissionService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var success = await _userService.RegisterAsync(request);
        return success ? Created(string.Empty, null) : Conflict("Bu e-posta adresi zaten kayıtlı.");
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized();

        var roles          = await _userService.GetUserRolesAsync(user.Id);
        var hasAdminAccess = await _permissionService.HasPermissionAsync(user.Id, "admin_access");
        var token          = _tokenService.GenerateToken(user.Id.ToString(), user.Email, roles, user.TeamId, hasAdminAccess);
        return Ok(new LoginResponseDto(token));
    }
}