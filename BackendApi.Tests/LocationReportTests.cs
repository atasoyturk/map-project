using System.Net;
using System.Net.Http.Json;
using BackendApi.DTOs.Analysis;
using BackendApi.Entities.Poi;
using BackendApi.Entities.Transit;
using BackendApi.Services.Analysis;
using BackendApi.Tests.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace BackendApi.Tests;

// Center (29, 41), bands 5/10/15 min. With a 562.5 s straight-line ray every ray reaches the full search radius
// at 15 min (~0.321° north), 2/3 of it at 10 min (~0.214°) and 1/3 at 5 min (~0.107°).
// Seeded categories: 1 "Yeme İçme" > 2 "Restoran", 3 "Kafe"; 10 "Eğitim" > 12 "Okul", 13 "Üniversite".
public sealed class LocationReportTests : ApiTestBase
{
    private const double CenterLon = 29, CenterLat = 41;
    private const double RaySeconds = 15 * 60 / 1.6;

    private readonly FakeOsrmService _osrm;
    private readonly MemoryCache     _cache;

    public LocationReportTests(ApiFixture fixture) : base(fixture)
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

    private static Task<HttpResponseMessage> PostAsync(HttpClient client, int competitorCategoryId, params int[] complementCategoryIds)
        => client.PostAsJsonAsync("/api/analysis/location-report",
            new LocationReportRequestDto(CenterLon, CenterLat, competitorCategoryId, complementCategoryIds));

    [Fact]
    public async Task LocationReport_CountsPerBand_NearestInMeters_AndScore()
    {
        _osrm.StraightLineDurationSeconds = RaySeconds;
        var (userId, client) = await CreateUserAsync([], ["location_analysis"]);

        await WithDbAsync(async db =>
        {
            db.Pois.AddRange(
                new Poi { Name = "Kafe A", UserId = userId, CategoryId = 3,  Geometry = Pt(29, 41.05) },                  // 5 min
                new Poi { Name = "Kafe B", UserId = userId, CategoryId = 3,  Geometry = Pt(29, 41.15) },                  // 10 min
                new Poi { Name = "Kafe C", UserId = userId, CategoryId = 3,  Geometry = Pt(29, 41.25) },                  // 15 min
                new Poi { Name = "Kafe D", UserId = userId, CategoryId = 3,  Geometry = Pt(29, 41.40) },                  // outside, 4th nearest
                new Poi { Name = "Kafe X", UserId = userId, CategoryId = 3,  Geometry = Pt(29, 41.01), IsActive = false },
                new Poi { Name = "Rest.",  UserId = userId, CategoryId = 2,  Geometry = Pt(29, 41.01) },                  // not a competitor
                new Poi { Name = "Okul 1", UserId = userId, CategoryId = 12, Geometry = Pt(29, 40.95) },                  // 5 min
                new Poi { Name = "Üni",    UserId = userId, CategoryId = 13, Geometry = Pt(29, 40.85) },                  // 10 min
                new Poi { Name = "Okul 2", UserId = userId, CategoryId = 12, Geometry = Pt(29, 40.75) },                  // 15 min
                new Poi { Name = "Okul 3", UserId = userId, CategoryId = 12, Geometry = Pt(29, 40.50) });                 // outside

            var route = new TransitRoute { Name = "Hat 1", Color = "#ff0000" };
            db.TransitRoutes.Add(route);
            await db.SaveChangesAsync();

            db.TransitStops.AddRange(
                new TransitStop { Name = "D1", TransitRouteId = route.Id, UserId = userId, Geometry = Pt(29, 41.0018) },               // ~200 m
                new TransitStop { Name = "D2", TransitRouteId = route.Id, UserId = userId, Geometry = Pt(29, 41.0001), IsDeleted = true });
            await db.SaveChangesAsync();
        });

        var response = await PostAsync(client, 3, 10);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = (await response.Content.ReadFromJsonAsync<LocationReportDto>())!;

        Assert.Equal([5, 10, 15], report.Bands.Select(b => b.MinutesThreshold));
        Assert.Equal([1, 2, 3], report.Bands.Select(b => b.CompetitorCount));
        Assert.Equal([1, 2, 3], report.Bands.Select(b => b.ComplementCount));
        Assert.Equal([1, 1, 1], report.Bands.Select(b => b.StopCount));
        Assert.All(report.Bands, b => Assert.Equal((10, "Eğitim"), (b.ComplementCounts.Single().CategoryId, b.ComplementCounts.Single().CategoryName)));
        Assert.True(report.Bands[0].AreaSquareMeters > 1e8);
        Assert.True(report.Bands[2].AreaSquareMeters > report.Bands[1].AreaSquareMeters);

        Assert.Equal(["Kafe A", "Kafe B", "Kafe C"], report.NearestCompetitors.Select(c => c.Name));
        Assert.InRange(report.NearestCompetitors[0].DistanceMeters, 5500, 5600); // 0.05° of latitude, in meters
        Assert.Equal("D1", report.NearestStops.Single().Name);
        Assert.InRange(report.NearestStops[0].DistanceMeters, 190, 210);

        // complements 1/2/3, competitors 2 and 1 stop at ~200 m within 10 min → see LocationScoreTests.
        Assert.Equal(LocationScore.Compute(1, 2, 3, 2, report.NearestStops[0].DistanceMeters, 1), report.Score);
        Assert.Equal(36, report.Score.Total);
    }

    [Fact]
    public async Task LocationReport_NearestCompetitors_AreRankedByTrueDistanceNotDegrees()
    {
        _osrm.StraightLineDurationSeconds = RaySeconds;
        var (userId, client) = await CreateUserAsync([], ["location_analysis"]);

        // At 41° N a degree of longitude is ~0.75 of a degree of latitude:
        // "North" is nearer in degrees (0.009 vs 0.010) but farther in meters (~1000 m vs ~840 m).
        await WithDbAsync(async db =>
        {
            db.Pois.AddRange(
                new Poi { Name = "North", UserId = userId, CategoryId = 3, Geometry = Pt(29,     41.009) },
                new Poi { Name = "East",  UserId = userId, CategoryId = 3, Geometry = Pt(29.010, 41) });
            await db.SaveChangesAsync();
        });

        var response = await PostAsync(client, 3, 10);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = (await response.Content.ReadFromJsonAsync<LocationReportDto>())!;

        Assert.Equal(["East", "North"], report.NearestCompetitors.Select(c => c.Name));
        Assert.InRange(report.NearestCompetitors[0].DistanceMeters, 830, 850);
        Assert.InRange(report.NearestCompetitors[1].DistanceMeters, 990, 1010);
    }

    [Fact]
    public async Task LocationReport_CompetitorOverlapsComplement_Returns400()
    {
        _osrm.StraightLineDurationSeconds = RaySeconds;
        var (_, client) = await CreateUserAsync([], ["location_analysis"]);

        var response = await PostAsync(client, 3, 1); // "Kafe" is inside "Yeme İçme"

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, _osrm.DurationCallCount);
    }

    [Fact]
    public async Task LocationReport_UnknownCategory_Returns400()
    {
        var (_, client) = await CreateUserAsync([], ["location_analysis"]);

        var response = await PostAsync(client, 3, 9999);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LocationReport_CenterOutOfRange_Returns400WithoutOsrmCall()
    {
        var (_, client) = await CreateUserAsync([], ["location_analysis"]);

        var response = await client.PostAsJsonAsync("/api/analysis/location-report",
            new LocationReportRequestDto(200, CenterLat, 3, [10]));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, _osrm.DurationCallCount);
    }

    [Fact]
    public async Task LocationReport_OsrmFails_Returns409()
    {
        var (_, client) = await CreateUserAsync([], ["location_analysis"]);

        var response = await PostAsync(client, 3, 10);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task LocationReport_WithoutPermission_Returns403()
    {
        var (_, client) = await CreateUserAsync([], ["transit_stop_read"]);

        var response = await PostAsync(client, 3, 10);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
