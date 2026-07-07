using Microsoft.AspNetCore.Mvc;
using BackendApi.DTOs;
using BackendApi.Repositories;
using BackendApi.Services.Auth;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserRepository         _userRepository;
    private readonly IUserService            _userService;
    private readonly ITokenService           _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository         userRepository,
        IUserService            userService,
        ITokenService           tokenService,
        ILogger<AuthController> logger)
    {
        _userRepository = userRepository;
        _userService    = userService;
        _tokenService   = tokenService;
        _logger         = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var success = await _userService.RegisterAsync(request);
            return success ? Created(string.Empty, null) : Conflict("Bu e-posta adresi zaten kayıtlı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Register failed");
            return StatusCode(500, "Sunucu hatası.");
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized();

            var roles = await _userService.GetUserRolesAsync(user.Id);
            var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email, roles);
            return Ok(new LoginResponseDto(token));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed");
            return StatusCode(500, "Sunucu hatası.");
        }
    }
}