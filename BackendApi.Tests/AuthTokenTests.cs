using System.IdentityModel.Tokens.Jwt;
using BackendApi.Tests.Infrastructure;

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
}
