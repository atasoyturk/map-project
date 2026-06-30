using Microsoft.AspNetCore.Mvc;
using BackendApi.DTOs;
using BackendApi.Repositories;
using BackendApi.Services;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthController(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) // user enumuration attack guard
            return Unauthorized();

        var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email, user.Role);

        return Ok(new LoginResponseDto(token));
    }
}