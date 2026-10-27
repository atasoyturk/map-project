using System.Net;
using System.Net.Http.Json;
using BackendApi.DTOs.Poi;
using BackendApi.DTOs.Transit;
using BackendApi.Entities.Transit;
using BackendApi.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Tests;

// Current behaviour: a non-owner without admin_access gets 404 (not 403) on edit/delete.
public sealed class PoiAndStopOwnershipTests : ApiTestBase
{
    public PoiAndStopOwnershipTests(ApiFixture fixture) : base(fixture) { }

    private static object Poi(string name) => new { name, workingHours = "09-18", wktGeometry = "POINT(29 41)", categoryId = 2 };

    private static object Stop(string name, int routeId) => new { name, wktGeometry = "POINT(29 41)", transitRouteId = routeId };

    private static async Task<int> CreatePoiAsync(HttpClient owner)
    {
        var response = await owner.PostAsJsonAsync("/api/poi", Poi("İlk ad"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PoiResponseDto>())!.Id;
    }

    private async Task<int> CreateRouteAsync()
    {
        var routeId = 0;
        await WithDbAsync(async db =>
        {
            var route = new TransitRoute { Name = "Hat 1", Color = "#ff0000" };
            db.TransitRoutes.Add(route);
            await db.SaveChangesAsync();
            routeId = route.Id;
        });
        return routeId;
    }

    private static async Task<int> CreateStopAsync(HttpClient owner, int routeId)
    {
        var response = await owner.PostAsJsonAsync("/api/transit-stop", Stop("İlk durak", routeId));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TransitStopResponseDto>())!.Id;
    }

    [Theory]
    [InlineData("owner", HttpStatusCode.OK)]
    [InlineData("admin", HttpStatusCode.OK)]
    [InlineData("other", HttpStatusCode.NotFound)]
    public async Task UpdatePoi(string actor, HttpStatusCode expected)
    {
        var clients = await CreateActorsAsync(Roles.PoiOperator);
        var poiId   = await CreatePoiAsync(clients["owner"]);

        var response = await clients[actor].PutAsJsonAsync($"/api/poi/{poiId}", Poi("Yeni ad"));

        Assert.Equal(expected, response.StatusCode);
        if (actor == "other") await AssertPoiUnchangedAsync(poiId);
    }

    [Theory]
    [InlineData("owner", HttpStatusCode.NoContent)]
    [InlineData("admin", HttpStatusCode.NoContent)]
    [InlineData("other", HttpStatusCode.NotFound)]
    public async Task DeletePoi(string actor, HttpStatusCode expected)
    {
        var clients = await CreateActorsAsync(Roles.PoiOperator);
        var poiId   = await CreatePoiAsync(clients["owner"]);

        var response = await clients[actor].DeleteAsync($"/api/poi/{poiId}");

        Assert.Equal(expected, response.StatusCode);
        if (actor == "other") await AssertPoiUnchangedAsync(poiId);
    }

    [Theory]
    [InlineData("owner", HttpStatusCode.OK)]
    [InlineData("admin", HttpStatusCode.OK)]
    [InlineData("other", HttpStatusCode.NotFound)]
    public async Task UpdateStop(string actor, HttpStatusCode expected)
    {
        var clients = await CreateActorsAsync(Roles.TransitOperator);
        var routeId = await CreateRouteAsync();
        var stopId  = await CreateStopAsync(clients["owner"], routeId);

        var response = await clients[actor].PutAsJsonAsync($"/api/transit-stop/{stopId}", Stop("Yeni durak", routeId));

        Assert.Equal(expected, response.StatusCode);
        if (actor == "other") await AssertStopUnchangedAsync(stopId);
    }

    [Theory]
    [InlineData("owner", HttpStatusCode.NoContent)]
    [InlineData("admin", HttpStatusCode.NoContent)]
    [InlineData("other", HttpStatusCode.NotFound)]
    public async Task DeleteStop(string actor, HttpStatusCode expected)
    {
        var clients = await CreateActorsAsync(Roles.TransitOperator);
        var routeId = await CreateRouteAsync();
        var stopId  = await CreateStopAsync(clients["owner"], routeId);

        var response = await clients[actor].DeleteAsync($"/api/transit-stop/{stopId}");

        Assert.Equal(expected, response.StatusCode);
        if (actor == "other") await AssertStopUnchangedAsync(stopId);
    }

    private Task AssertPoiUnchangedAsync(int poiId) => WithDbAsync(async db =>
    {
        var poi = await db.Pois.SingleAsync(p => p.Id == poiId);
        Assert.False(poi.IsDeleted);
        Assert.Equal("İlk ad", poi.Name);
    });

    private Task AssertStopUnchangedAsync(int stopId) => WithDbAsync(async db =>
    {
        var stop = await db.TransitStops.SingleAsync(s => s.Id == stopId);
        Assert.False(stop.IsDeleted);
        Assert.Equal("İlk durak", stop.Name);
    });

    // Owner and "other" share the same operator role, so only ownership differs.
    private async Task<Dictionary<string, HttpClient>> CreateActorsAsync(int operatorRoleId)
    {
        var (_, owner) = await CreateUserAsync([operatorRoleId]);
        var (_, other) = await CreateUserAsync([operatorRoleId]);
        var (_, admin) = await CreateUserAsync([Roles.Admin]);
        return new() { ["owner"] = owner, ["other"] = other, ["admin"] = admin };
    }
}
