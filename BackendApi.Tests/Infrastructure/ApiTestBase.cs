using System.Net.Http.Headers;
using System.Net.Http.Json;
using BackendApi.Data;
using BackendApi.DTOs.Auth;
using BackendApi.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendApi.Tests.Infrastructure;

// Seeded role IDs from AppDbContext.
public static class Roles
{
    public const int Admin            = 1;
    public const int Intern           = 3;
    public const int PoiOperator      = 5;
    public const int BasicUser        = 6;
    public const int TransitOperator  = 7;
}

[Collection(ApiCollection.Name)]
public abstract class ApiTestBase : IAsyncLifetime
{
    private const string Password = "Test1234!";

    private readonly ApiFixture _fixture;
    private int _userCounter;

    protected ApiTestBase(ApiFixture fixture) => _fixture = fixture;

    public virtual Task InitializeAsync() => _fixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    protected HttpClient CreateAnonymousClient() => _fixture.Factory.CreateClient();

    protected async Task WithDbAsync(Func<AppDbContext, Task> action)
    {
        using var scope = _fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await action(db);
    }

    // Creates a user with the given roles, direct permissions and team, logs in, returns an authenticated client.
    protected async Task<(int UserId, HttpClient Client)> CreateUserAsync(
        int[]     roleIds,
        string[]? directPermissions = null,
        int?      teamId            = null)
    {
        var email = $"user{Interlocked.Increment(ref _userCounter)}@test.local";
        var userId = 0;

        await WithDbAsync(async db =>
        {
            var user = new User { Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password, workFactor: 4), TeamId = teamId };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            userId = user.Id;

            foreach (var roleId in roleIds)
                db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });

            foreach (var name in directPermissions ?? [])
            {
                var permission = await db.Permissions.SingleAsync(p => p.Name == name);
                db.UserPermissions.Add(new UserPermission { UserId = user.Id, PermissionId = permission.Id });
            }

            await db.SaveChangesAsync();
        });

        var client = CreateAnonymousClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password = Password });
        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);
        return (userId, client);
    }
}
