using BackendApi.DTOs;

namespace BackendApi.Services.Auth;

public interface IUserService
{
    Task<bool>          RegisterAsync(RegisterRequestDto request);
    Task<IList<string>> GetUserRolesAsync(int userId);
}