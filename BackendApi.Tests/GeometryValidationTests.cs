using System.Net;
using System.Net.Http.Json;
using BackendApi.DTOs.Analysis;
using BackendApi.Tests.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace BackendApi.Tests;

public sealed class GeometryValidationTests : ApiTestBase
{
    private const string BowTie = "POLYGON((29 41, 29.1 41.1, 29.1 41, 29 41.1, 29 41))";

    private readonly FakeOsrmService _osrm;
    private readonly MemoryCache     _cache;

    public GeometryValidationTests(ApiFixture fixture) : base(fixture)
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

    [Theory]
    [InlineData("/api/polygon")]
    [InlineData("/api/geo-permission")]
    public async Task Create_SelfIntersectingPolygon_Returns400(string url)
    {
        var (_, admin) = await CreateUserAsync([Roles.Admin]);

        var response = await admin.PostAsJsonAsync(url, new { name = "Papyon", color = "#ff0000", wktGeometry = BowTie });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Geometri geçersiz", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task IsochroneAnalysis_CrossingRays_ReturnsValidBandPolygon()
    {
        // Northern rays end mirrored through the center, so the ring built from the ray ends crosses itself.
        _osrm.StraightLineDurationSeconds = 10 * 60 / 1.6;
        _osrm.RayEnd = (from, to) => to.Y > from.Y ? new Coordinate(2 * from.X - to.X, 2 * from.Y - to.Y) : to;
        var (_, client) = await CreateUserAsync([], ["location_analysis"]);

        var response = await client.PostAsJsonAsync("/api/analysis/isochrone", new IsochroneRequestDto(29, 41, [10]));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = (await response.Content.ReadFromJsonAsync<IsochroneAnalysisResultDto>())!;
        var band   = new WKTReader().Read(Assert.Single(result.Bands).PolygonWkt);
        Assert.True(band.IsValid);
        Assert.IsAssignableFrom<IPolygonal>(band);
    }
}
