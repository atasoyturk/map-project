using System.Net;
using System.Net.Http.Json;
using BackendApi.DTOs.Geo;
using BackendApi.Entities.Geo;
using BackendApi.Tests.Infrastructure;

namespace BackendApi.Tests;

public sealed class GeoPermissionTests : ApiTestBase
{
    private const string Boundary     = "POLYGON((29 41, 29.1 41, 29.1 41.1, 29 41.1, 29 41))";
    private const string InsidePoint  = "POINT(29.05 41.05)";
    private const string OutsidePoint = "POINT(30 42)";

    public GeoPermissionTests(ApiFixture fixture) : base(fixture) { }

    private static object Poi(string wkt) => new { name = "Test POI", workingHours = "09-18", wktGeometry = wkt, categoryId = 2 };

    private static async Task<int> CreateBoundaryAsync(HttpClient admin)
    {
        var response = await admin.PostAsJsonAsync("/api/geo-permission", new { name = "Test sınırı", wktGeometry = Boundary });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<GeoPermissionResponseDto>())!.Id;
    }

    [Theory]
    [InlineData(InsidePoint,  HttpStatusCode.Created)]
    [InlineData(OutsidePoint, HttpStatusCode.Forbidden)]
    public async Task UserBoundary_LimitsWherePoiCanBeCreated(string wkt, HttpStatusCode expected)
    {
        var (_, admin)        = await CreateUserAsync([Roles.Admin]);
        var (userId, client)  = await CreateUserAsync([Roles.PoiOperator]);
        var boundaryId        = await CreateBoundaryAsync(admin);
        (await admin.PostAsync($"/api/geo-permission/user/{userId}/{boundaryId}", null)).EnsureSuccessStatusCode();

        var response = await client.PostAsJsonAsync("/api/poi", Poi(wkt));

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData(InsidePoint,  HttpStatusCode.Created)]
    [InlineData(OutsidePoint, HttpStatusCode.Forbidden)]
    public async Task RoleBoundary_LimitsWherePoiCanBeCreated(string wkt, HttpStatusCode expected)
    {
        var (_, admin)  = await CreateUserAsync([Roles.Admin]);
        var boundaryId  = await CreateBoundaryAsync(admin);
        (await admin.PostAsync($"/api/geo-permission/role/{Roles.PoiOperator}/{boundaryId}", null)).EnsureSuccessStatusCode();
        // Log in after the assignment; role boundaries are read per request, but keep the order realistic.
        var (_, client) = await CreateUserAsync([Roles.PoiOperator]);

        var response = await client.PostAsJsonAsync("/api/poi", Poi(wkt));

        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task UserWithoutAnyBoundary_CanCreateAnywhere()
    {
        var (_, client) = await CreateUserAsync([Roles.PoiOperator]);

        var response = await client.PostAsJsonAsync("/api/poi", Poi(OutsidePoint));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Admin_IsExemptFromBoundary()
    {
        var (adminId, admin) = await CreateUserAsync([Roles.Admin]);
        var boundaryId       = await CreateBoundaryAsync(admin);
        // The API refuses this assignment, so insert it directly to prove the exemption itself.
        await WithDbAsync(async db =>
        {
            db.UserGeoPermissions.Add(new UserGeoPermission { UserId = adminId, GeoPermissionId = boundaryId });
            await db.SaveChangesAsync();
        });

        var response = await admin.PostAsJsonAsync("/api/poi", Poi(OutsidePoint));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AssigningBoundaryToAdminUser_Returns403()
    {
        var (_, admin)        = await CreateUserAsync([Roles.Admin]);
        var (otherAdminId, _) = await CreateUserAsync([Roles.Admin]);
        var boundaryId        = await CreateBoundaryAsync(admin);

        var response = await admin.PostAsync($"/api/geo-permission/user/{otherAdminId}/{boundaryId}", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AssigningBoundaryToAdminRole_Returns403()
    {
        var (_, admin) = await CreateUserAsync([Roles.Admin]);
        var boundaryId = await CreateBoundaryAsync(admin);

        var response = await admin.PostAsync($"/api/geo-permission/role/{Roles.Admin}/{boundaryId}", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
