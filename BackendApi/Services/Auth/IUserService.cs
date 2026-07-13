using BackendApi.DTOs.Auth;

namespace BackendApi.Services.Auth;

public interface IUserService
{
    Task<bool>          RegisterAsync(RegisterRequestDto request);
    Task<IList<string>> GetUserRolesAsync(int userId);
    Task<IList<UserLookupDto>> GetLookupAsync();
}