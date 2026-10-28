using System.Net;
using System.Net.Http.Json;
using BackendApi.DTOs.Transit;
using BackendApi.Entities.Transit;
using BackendApi.Tests.Infrastructure;
using NetTopologySuite.Geometries;

namespace BackendApi.Tests;

// At lat 41, 0.001° lon ≈ 84 m and 0.05° ≈ 4.2 km.
public sealed class TripPlanningTests : ApiTestBase
{
    public TripPlanningTests(ApiFixture fixture) : base(fixture) { }

    private int _ownerId;
    private HttpClient _client = null!;

    // Stops need an owning user (FK), so the planner's account doubles as the owner.
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        (_ownerId, _client) = await CreateUserAsync([Roles.BasicUser]);
    }

    private async Task SeedRouteAsync(string name, params (double Lon, double Lat)[] stops)
    {
        await WithDbAsync(async db =>
        {
            var route = new TransitRoute { Name = name, Color = "#0000ff" };
            db.TransitRoutes.Add(route);
            await db.SaveChangesAsync();

            for (var i = 0; i < stops.Length; i++)
                db.TransitStops.Add(new TransitStop
                {
                    Name           = $"{name}-{i + 1}",
                    Geometry       = new Point(stops[i].Lon, stops[i].Lat) { SRID = 4326 },
                    TransitRouteId = route.Id,
                    UserId         = _ownerId,
                    SortOrder      = i,
                });
            await db.SaveChangesAsync();
        });
    }

    private Task<HttpResponseMessage> PlanAsync(double startLon, double startLat, double endLon, double endLat)
        => _client.PostAsJsonAsync("/api/trip-planning/plan", new TripPlanRequestDto(startLon, startLat, endLon, endLat));

    private static async Task<string[]> LegTypesAsync(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var plan = await response.Content.ReadFromJsonAsync<TripPlanResultDto>();
        return plan!.Legs.Select(l => l.Type == "Bus" ? $"Bus:{l.RouteName}" : l.Type).ToArray();
    }

    [Fact]
    public async Task ShortDistance_NoStopsNearby_IsWalkOnly()
    {
        await SeedRouteAsync("A", (30.00, 41.00), (30.05, 41.00));

        var response = await PlanAsync(29.000, 41.0, 29.003, 41.0);

        Assert.Equal(["Walk"], await LegTypesAsync(response));
    }

    [Fact]
    public async Task StopsNearStartAndEnd_UsesBus()
    {
        await SeedRouteAsync("A", (29.00, 41.00), (29.05, 41.00), (29.10, 41.00));

        var response = await PlanAsync(28.999, 41.0, 29.101, 41.0);

        Assert.Equal(["Walk", "Bus:A", "Walk"], await LegTypesAsync(response));
    }

    [Fact]
    public async Task NearbyStopsOnDifferentRoutes_Transfer()
    {
        await SeedRouteAsync("A", (29.00, 41.00), (29.05, 41.00));
        await SeedRouteAsync("B", (29.0505, 41.00), (29.0505, 41.05));

        var response = await PlanAsync(28.999, 41.0, 29.0505, 41.051);

        Assert.Equal(["Walk", "Bus:A", "Walk", "Bus:B", "Walk"], await LegTypesAsync(response));
    }

    [Fact]
    public async Task BusOnlyRunsInStopOrder()
    {
        await SeedRouteAsync("A", (29.00, 41.00), (29.05, 41.00), (29.10, 41.00));

        // Reverse direction: no bus edges, so the planner falls back to a direct walk.
        var response = await PlanAsync(29.101, 41.0, 28.999, 41.0);

        Assert.Equal(["Walk"], await LegTypesAsync(response));
    }

    // Current behaviour: a direct start→end walk edge is always added, so the planner never
    // returns null and the endpoint never returns 404, even for points ~350 km apart.
    [Fact]
    public async Task FarApartPoints_WithNoStops_StillReturnWalk_Never404()
    {
        var response = await PlanAsync(28.97, 41.01, 32.85, 39.93);

        Assert.Equal(["Walk"], await LegTypesAsync(response));
    }
}
