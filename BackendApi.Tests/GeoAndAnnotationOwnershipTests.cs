using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BackendApi.Tests.Infrastructure;

namespace BackendApi.Tests;

// Admin is detected by the admin_access permission, not by the "Admin" role name.
// "permAdmin" has admin_access as a direct permission and no Admin role.
public sealed class GeoAndAnnotationOwnershipTests : ApiTestBase
{
    public GeoAndAnnotationOwnershipTests(ApiFixture fixture) : base(fixture) { }

    private static readonly Dictionary<string, string> Wkt = new()
    {
        ["point"]   = "POINT(29 41)",
        ["line"]    = "LINESTRING(29 41, 29.1 41.1)",
        ["polygon"] = "POLYGON((29 41, 29.1 41, 29.1 41.1, 29 41))"
    };

    private static object Shape(string type, string name) => new { name, color = "#ff0000", wktGeometry = Wkt[type] };

    private static async Task<int> ReadIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetInt32();
    }

    [Theory]
    [InlineData("point",   "owner",     HttpStatusCode.OK)]
    [InlineData("point",   "admin",     HttpStatusCode.OK)]
    [InlineData("point",   "permAdmin", HttpStatusCode.OK)]
    [InlineData("point",   "other",     HttpStatusCode.NotFound)]
    [InlineData("line",    "permAdmin", HttpStatusCode.OK)]
    [InlineData("line",    "other",     HttpStatusCode.NotFound)]
    [InlineData("polygon", "permAdmin", HttpStatusCode.OK)]
    [InlineData("polygon", "other",     HttpStatusCode.NotFound)]
    public async Task UpdateShape(string type, string actor, HttpStatusCode expected)
    {
        var clients = await CreateActorsAsync();
        var id      = await ReadIdAsync(await clients["owner"].PostAsJsonAsync($"/api/{type}", Shape(type, "İlk ad")));

        var response = await clients[actor].PutAsJsonAsync($"/api/{type}/{id}", Shape(type, "Yeni ad"));

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("point",   "owner",     HttpStatusCode.NoContent)]
    [InlineData("point",   "admin",     HttpStatusCode.NoContent)]
    [InlineData("point",   "permAdmin", HttpStatusCode.NoContent)]
    [InlineData("point",   "other",     HttpStatusCode.NotFound)]
    [InlineData("line",    "permAdmin", HttpStatusCode.NoContent)]
    [InlineData("line",    "other",     HttpStatusCode.NotFound)]
    [InlineData("polygon", "permAdmin", HttpStatusCode.NoContent)]
    [InlineData("polygon", "other",     HttpStatusCode.NotFound)]
    public async Task DeleteShape(string type, string actor, HttpStatusCode expected)
    {
        var clients = await CreateActorsAsync();
        var id      = await ReadIdAsync(await clients["owner"].PostAsJsonAsync($"/api/{type}", Shape(type, "İlk ad")));

        var response = await clients[actor].DeleteAsync($"/api/{type}/{id}");

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("point",   "owner",     HttpStatusCode.OK)]
    [InlineData("point",   "admin",     HttpStatusCode.OK)]
    [InlineData("point",   "permAdmin", HttpStatusCode.OK)]
    [InlineData("point",   "other",     HttpStatusCode.NotFound)]
    [InlineData("line",    "permAdmin", HttpStatusCode.OK)]
    [InlineData("line",    "other",     HttpStatusCode.NotFound)]
    [InlineData("polygon", "permAdmin", HttpStatusCode.OK)]
    [InlineData("polygon", "other",     HttpStatusCode.NotFound)]
    public async Task GetShapeById(string type, string actor, HttpStatusCode expected)
    {
        var clients = await CreateActorsAsync();
        var id      = await ReadIdAsync(await clients["owner"].PostAsJsonAsync($"/api/{type}", Shape(type, "İlk ad")));

        var response = await clients[actor].GetAsync($"/api/{type}/{id}");

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("owner",     HttpStatusCode.NoContent)]
    [InlineData("admin",     HttpStatusCode.NoContent)]
    [InlineData("permAdmin", HttpStatusCode.NoContent)]
    [InlineData("other",     HttpStatusCode.NotFound)]
    public async Task DeleteAnnotation(string actor, HttpStatusCode expected)
    {
        var clients = await CreateActorsAsync();
        var id      = await ReadIdAsync(await clients["owner"].PostAsJsonAsync("/api/annotation",
            new { noteText = "Not", wktGeometry = "POINT(29 41)" }));

        var response = await clients[actor].DeleteAsync($"/api/annotation/{id}");

        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task AdminUserList_FlagsUsersWithAdminAccess()
    {
        var (adminId, admin) = await CreateUserAsync([Roles.Admin]);
        var (permAdminId, _) = await CreateUserAsync([], ["admin_access"]);
        var (plainId, _)     = await CreateUserAsync([Roles.Intern]);

        var users = await admin.GetFromJsonAsync<JsonElement>("/api/admin/users");
        bool Flag(int id) => users.EnumerateArray()
            .Single(u => u.GetProperty("id").GetInt32() == id)
            .GetProperty("hasAdminAccess").GetBoolean();

        Assert.True(Flag(adminId));
        Assert.True(Flag(permAdminId));
        Assert.False(Flag(plainId));
    }

    [Fact]
    public async Task AdminRoleList_FlagsRolesWithAdminAccess()
    {
        var (_, admin) = await CreateUserAsync([Roles.Admin]);

        var roles = await admin.GetFromJsonAsync<JsonElement>("/api/admin/roles");
        var flagged = roles.EnumerateArray()
            .Where(r => r.GetProperty("hasAdminAccess").GetBoolean())
            .Select(r => r.GetProperty("id").GetInt32());

        Assert.Equal([Roles.Admin], flagged);
    }

    // Owner and "other" share the same role, so only ownership differs.
    private async Task<Dictionary<string, HttpClient>> CreateActorsAsync()
    {
        var (_, owner)     = await CreateUserAsync([Roles.Intern]);
        var (_, other)     = await CreateUserAsync([Roles.Intern]);
        var (_, admin)     = await CreateUserAsync([Roles.Admin]);
        var (_, permAdmin) = await CreateUserAsync([], ["admin_access"]);
        return new() { ["owner"] = owner, ["other"] = other, ["admin"] = admin, ["permAdmin"] = permAdmin };
    }
}
