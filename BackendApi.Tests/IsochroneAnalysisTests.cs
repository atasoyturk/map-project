using System.Net;
using System.Net.Http.Json;
using BackendApi.DTOs.Analysis;
using BackendApi.Entities.Poi;
using BackendApi.Entities.Transit;
using BackendApi.Tests.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace BackendApi.Tests;

// Center (29, 41), thresholds 5 and 10 min. With a 375 s straight-line ray every ray reaches the full search
// radius at 10 min (~0.214° north) and half of it at 5 min (~0.107° north). Seeded categories: 3 "Kafe", 6 "Hastane".
public sealed class IsochroneAnalysisTests : ApiTestBase
{
    private const double CenterLon = 29, CenterLat = 41;
    private const double RaySeconds = 10 * 60 / 1.6;

    private readonly FakeOsrmService _osrm;
    private readonly MemoryCache     _cache;

    public IsochroneAnalysisTests(ApiFixture fixture) : base(fixture)
    {
        _osrm  = fixture.Factory.Services.GetRequiredService<FakeOsrmService>();
        _cache = (MemoryCache)fixture.Factory.Services.GetRequiredService<IMemoryCache>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _osrm.Reset();
        _cache.Clear();
    }

    private static Point Pt(double lon, double lat) => new(lon, lat) { SRID = 4326 };

    private static Task<HttpResponseMessage> PostAsync(HttpClient client, params int[] minutes)
        => client.PostAsJsonAsync("/api/analysis/isochrone", new IsochroneRequestDto(CenterLon, CenterLat, minutes));

    private static async Task<IsochroneAnalysisResultDto> AnalyseAsync(HttpClient client, params int[] minutes)
    {
        var response = await PostAsync(client, minutes);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<IsochroneAnalysisResultDto>())!;
    }

    [Fact]
    public async Task IsochroneAnalysis_CountsActivePoisAndStopsCumulativelyPerBand()
    {
        _osrm.StraightLineDurationSeconds = RaySeconds;
        var (userId, client) = await CreateUserAsync([], ["location_analysis"]);

        await WithDbAsync(async db =>
        {
            db.Pois.AddRange(
                new Poi { UserId = userId, CategoryId = 3, Geometry = Pt(29, 41.05) },                     // within 5 and 10
                new Poi { UserId = userId, CategoryId = 6, Geometry = Pt(29, 40.95) },                     // within 5 and 10
                new Poi { UserId = userId, CategoryId = 3, Geometry = Pt(29, 41.15) },                     // within 10 only
                new Poi { UserId = userId, CategoryId = 3, Geometry = Pt(29, 41.30) },                     // outside
                new Poi { UserId = userId, CategoryId = 3, Geometry = Pt(29, 41.05), IsDeleted = true },   // deleted
                new Poi { UserId = userId, CategoryId = 3, Geometry = Pt(29, 41.05), IsActive  = false }); // inactive

            var route = new TransitRoute { Name = "Hat 1", Color = "#ff0000" };
            db.TransitRoutes.Add(route);
            await db.SaveChangesAsync();

            db.TransitStops.AddRange(
                new TransitStop { Name = "D1", TransitRouteId = route.Id, UserId = userId, Geometry = Pt(29, 41.02) },                   // within 5 and 10
                new TransitStop { Name = "D2", TransitRouteId = route.Id, UserId = userId, Geometry = Pt(29, 41.18) },                   // within 10 only
                new TransitStop { Name = "D3", TransitRouteId = route.Id, UserId = userId, Geometry = Pt(29, 41.02), IsDeleted = true }); // deleted
            await db.SaveChangesAsync();
        });

        var result = await AnalyseAsync(client, 5, 10);

        var five = result.Bands.Single(b => b.MinutesThreshold == 5);
        Assert.Equal(1, five.StopCount);
        Assert.Equal([(6, "Hastane", 1), (3, "Kafe", 1)], five.CategoryCounts.Select(c => (c.CategoryId, c.CategoryName, c.Count)));

        // Cumulative, not a ring: the 10-min band also holds everything within 5 min.
        var ten = result.Bands.Single(b => b.MinutesThreshold == 10);
        Assert.Equal(2, ten.StopCount);
        Assert.Equal([(3, "Kafe", 2), (6, "Hastane", 1)], ten.CategoryCounts.Select(c => (c.CategoryId, c.CategoryName, c.Count)));
    }

    [Fact]
    public async Task IsochroneAnalysis_SamePointTwice_CallsOsrmOnce()
    {
        _osrm.StraightLineDurationSeconds = RaySeconds;
        var (_, client) = await CreateUserAsync([], ["location_analysis"]);

        await AnalyseAsync(client, 10, 5);
        await AnalyseAsync(client, 5, 10);

        Assert.Equal(16, _osrm.DurationCallCount);
    }

    [Fact]
    public async Task IsochroneAnalysis_OsrmFails_ReturnsNoBandsAndIsNotCached()
    {
        var (_, client) = await CreateUserAsync([], ["location_analysis"]);

        Assert.Empty((await AnalyseAsync(client, 10)).Bands);
        Assert.Empty((await AnalyseAsync(client, 10)).Bands);

        Assert.Equal(32, _osrm.DurationCallCount);
    }

    [Fact]
    public async Task IsochroneAnalysis_WithoutPermission_Returns403()
    {
        var (_, client) = await CreateUserAsync([], ["transit_stop_read"]);

        var response = await PostAsync(client, 10);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
