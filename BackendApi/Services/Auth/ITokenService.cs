namespace BackendApi.Services.Auth;

public interface ITokenService
{
    string GenerateToken(string userId, string email, IEnumerable<string> roles, int? teamId);
}