using System.Net;
using System.Net.Http.Json;
using BackendApi.Tests.Infrastructure;

namespace BackendApi.Tests;

public sealed class PermissionTests : ApiTestBase
{
    private static readonly object ValidPoi = new { name = "Test POI", workingHours = "09-18", wktGeometry = "POINT(29 41)", categoryId = 2 };

    public PermissionTests(ApiFixture fixture) : base(fixture) { }

    // "Kullanıcı" role only has poi_read and transit_stop_read.
    [Theory]
    [InlineData("POST", "/api/poi")]
    [InlineData("POST", "/api/transit-stop")]
    [InlineData("POST", "/api/transit-route")]
    [InlineData("POST", "/api/geo-permission")]
    [InlineData("GET",  "/api/admin/users")]
    [InlineData("POST", "/api/analysis/location-analysis")]
    public async Task UserWithoutPermission_Gets403(string method, string url)
    {
        var (_, client) = await CreateUserAsync([Roles.BasicUser]);

        var request = new HttpRequestMessage(new HttpMethod(method), url);
        if (method == "POST") request.Content = JsonContent.Create(new { });
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UserWithPermission_CanCallEndpoint()
    {
        var (_, client) = await CreateUserAsync([Roles.BasicUser]);

        var response = await client.GetAsync("/api/poi");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DirectUserPermission_WithoutRole_IsEnough()
    {
        var (_, client) = await CreateUserAsync([], ["poi_create"]);

        var response = await client.PostAsJsonAsync("/api/poi", ValidPoi);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AnonymousRequest_Gets401()
    {
        var response = await CreateAnonymousClient().GetAsync("/api/poi");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
