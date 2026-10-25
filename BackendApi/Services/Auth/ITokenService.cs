namespace BackendApi.Services.Auth;

public interface ITokenService
{
    string GenerateToken(string userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions, int? teamId, bool hasAdminAccess);
}