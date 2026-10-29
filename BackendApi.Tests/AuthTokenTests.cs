using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using BackendApi.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Tests;

public sealed class AuthTokenTests : ApiTestBase
{
    private const int ExpectedLifetimeMinutes = 480;

    public AuthTokenTests(ApiFixture fixture) : base(fixture) { }

    // Token lifetime must not depend on role names (Admin used to get ~10 years).
    [Theory]
    [InlineData(Roles.Admin)]
    [InlineData(Roles.BasicUser)]
    public async Task Login_IssuesTokenWithConfiguredLifetime(int roleId)
    {
        var before = DateTime.UtcNow;
        var (_, client) = await CreateUserAsync([roleId]);
        var after = DateTime.UtcNow;

        var token = new JwtSecurityTokenHandler().ReadJwtToken(client.DefaultRequestHeaders.Authorization!.Parameter);

        // exp has second precision, so allow one second of truncation.
        Assert.InRange(token.ValidTo,
            before.AddMinutes(ExpectedLifetimeMinutes).AddSeconds(-1),
            after.AddMinutes(ExpectedLifetimeMinutes));
    }

    // Inactive and deleted users get the same bare 401 as a wrong password (no user enumeration).
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Login_DisabledUser_Gets401(bool deleted)
    {
        var (userId, _) = await CreateUserAsync([Roles.BasicUser]);
        var email = await DisableUserAsync(userId, deleted);

        var response = await CreateAnonymousClient().PostAsJsonAsync("/api/auth/login", new { email, password = Password });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // A token issued before deactivation must stop working on every authenticated endpoint.
    [Theory]
    [InlineData("/api/users/lookup")] // [Authorize] only
    [InlineData("/api/poi")]          // permission-protected
    public async Task ExistingToken_OfDeactivatedUser_Gets401(string url)
    {
        var (userId, client) = await CreateUserAsync([Roles.BasicUser]);
        await DisableUserAsync(userId, deleted: false);

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<string> DisableUserAsync(int userId, bool deleted)
    {
        var email = string.Empty;
        await WithDbAsync(async db =>
        {
            var user = await db.Users.SingleAsync(u => u.Id == userId);
            if (deleted) user.IsDeleted = true;
            else         user.IsActive  = false;
            await db.SaveChangesAsync();
            email = user.Email;
        });
        return email;
    }
}
