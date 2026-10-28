using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BackendApi.DTOs.Analysis;
using BackendApi.Entities.Geo;
using BackendApi.Entities.Poi;
using BackendApi.Tests.Infrastructure;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace BackendApi.Tests;

// Region is the square lon 29..29.1, lat 41..41.1. Seeded categories: 1 "Yeme İçme" → 2, 3, 4; 5 "Sağlık" → 6..9.
public sealed class AnalysisTests : ApiTestBase
{
    public AnalysisTests(ApiFixture fixture) : base(fixture) { }

    private const string Region = "POLYGON((29 41, 29.1 41, 29.1 41.1, 29 41.1, 29 41))";

    private static readonly WKTReader Reader = new();

    private static Geometry Geom(string wkt)
    {
        var geometry = Reader.Read(wkt);
        geometry.SRID = 4326;
        return geometry;
    }

    private static Point Pt(double lon, double lat) => new(lon, lat) { SRID = 4326 };

    // ---------- temp-inventory ----------

    private static async Task<int> TempInventoryAsync(HttpClient client, string wkt = Region)
    {
        var response = await client.PostAsJsonAsync("/api/analysis/temp-inventory", new { wktGeometry = wkt });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("intersectedInventoryCount").GetInt32();
    }

    [Fact]
    public async Task TempInventory_CountsOwnActiveFeaturesIntersectingRegion()
    {
        var (userId, client) = await CreateUserAsync([], ["area_scan"]);
        var (otherId, _)     = await CreateUserAsync([], ["area_scan"]);

        await WithDbAsync(async db =>
        {
            db.Points.AddRange(
                new PointEntity { UserId = userId,  Geometry = Pt(29.05, 41.05) },                       // counted
                new PointEntity { UserId = userId,  Geometry = Pt(29.50, 41.50) },                       // outside
                new PointEntity { UserId = userId,  Geometry = Pt(29.05, 41.05), IsDeleted = true },     // deleted
                new PointEntity { UserId = userId,  Geometry = Pt(29.05, 41.05), IsActive  = false },    // inactive
                new PointEntity { UserId = otherId, Geometry = Pt(29.05, 41.05) });                      // other user
            db.Lines.AddRange(
                new LineEntity { UserId = userId, Geometry = Geom("LINESTRING(28.9 41.05, 29.2 41.05)") }, // crosses: counted
                new LineEntity { UserId = userId, Geometry = Geom("LINESTRING(28.0 40.0, 28.1 40.1)") });  // outside
            db.Polygons.AddRange(
                new PolygonEntity { UserId = userId, Geometry = Geom("POLYGON((29.09 41.09, 29.2 41.09, 29.2 41.2, 29.09 41.2, 29.09 41.09))") }, // overlaps: counted
                new PolygonEntity { UserId = userId, Geometry = Geom("POLYGON((30 42, 30.1 42, 30.1 42.1, 30 42.1, 30 42))") });                 // outside
            await db.SaveChangesAsync();
        });

        Assert.Equal(3, await TempInventoryAsync(client));
    }

    [Fact]
    public async Task TempInventory_NoFeatures_ReturnsZero()
    {
        var (_, client) = await CreateUserAsync([], ["area_scan"]);

        Assert.Equal(0, await TempInventoryAsync(client));
    }

    // ---------- location-analysis ----------

    private static Task<HttpResponseMessage> LocationAnalysisAsync(HttpClient client, params (int CategoryId, int Score)[] criteria)
        => client.PostAsJsonAsync("/api/analysis/location-analysis", new LocationAnalysisRequestDto
        {
            WktGeometry = Region,
            Criteria    = criteria.Select(c => new LocationAnalysisCriterionDto { CategoryId = c.CategoryId, Score = c.Score }).ToList()
        });

    private static async Task<string[]> HeatmapAsync(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LocationAnalysisResponseDto>();
        return body!.HeatmapPoints
            .Select(p => FormattableString.Invariant($"{p.Longitude}/{p.Latitude}/{p.Weight}"))
            .Order()
            .ToArray();
    }

    private async Task SeedPoisAsync(int ownerId, params Poi[] pois)
    {
        foreach (var poi in pois) poi.UserId = ownerId;
        await WithDbAsync(async db =>
        {
            db.Pois.AddRange(pois);
            await db.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task LocationAnalysis_ReturnsWeightedPoisInRegionByCategoryAndDescendants()
    {
        var (userId, client) = await CreateUserAsync([], ["location_analysis"]);
        var (otherId, _)     = await CreateUserAsync([]);

        await SeedPoisAsync(userId,
            new Poi { CategoryId = 1, Geometry = Pt(29.01, 41.01) },                    // parent category itself
            new Poi { CategoryId = 2, Geometry = Pt(29.02, 41.02) },                    // child of 1
            new Poi { CategoryId = 6, Geometry = Pt(29.03, 41.03) },                    // child of 5
            new Poi { CategoryId = 2, Geometry = Pt(29.50, 41.50) },                    // outside region
            new Poi { CategoryId = 2, Geometry = Pt(29.04, 41.04), IsDeleted = true },  // deleted
            new Poi { CategoryId = 10, Geometry = Pt(29.05, 41.05) });                  // category not in criteria
        // POIs are shared reference data: other users' POIs are included.
        await SeedPoisAsync(otherId, new Poi { CategoryId = 3, Geometry = Pt(29.06, 41.06) });

        var response = await LocationAnalysisAsync(client, (1, 70), (5, 30));

        Assert.Equal(
            new[] { "29.01/41.01/0.7", "29.02/41.02/0.7", "29.03/41.03/0.3", "29.06/41.06/0.7" },
            await HeatmapAsync(response));
    }

    [Fact]
    public async Task LocationAnalysis_PoiMatchingTwoCriteria_IsReturnedOncePerCriterion()
    {
        var (userId, client) = await CreateUserAsync([], ["location_analysis"]);
        await SeedPoisAsync(userId, new Poi { CategoryId = 2, Geometry = Pt(29.01, 41.01) });

        var response = await LocationAnalysisAsync(client, (1, 60), (2, 40));

        Assert.Equal(new[] { "29.01/41.01/0.4", "29.01/41.01/0.6" }, await HeatmapAsync(response));
    }

    // Current behaviour: only IsDeleted is filtered, so inactive POIs are still included.
    [Fact]
    public async Task LocationAnalysis_InactivePoi_IsIncluded()
    {
        var (userId, client) = await CreateUserAsync([], ["location_analysis"]);
        await SeedPoisAsync(userId, new Poi { CategoryId = 2, Geometry = Pt(29.01, 41.01), IsActive = false });

        var response = await LocationAnalysisAsync(client, (1, 50), (5, 50));

        Assert.Equal(new[] { "29.01/41.01/0.5" }, await HeatmapAsync(response));
    }

    [Fact]
    public async Task LocationAnalysis_DeletedSubcategory_IsSkipped()
    {
        var (userId, client) = await CreateUserAsync([], ["location_analysis"]);

        // tbl_poi_category is not reset between tests, so remove the extra category afterwards.
        var deletedCategory = new PoiCategory { Name = "Deleted child", ParentCategoryId = 1, IsDeleted = true };
        await WithDbAsync(async db =>
        {
            db.PoiCategories.Add(deletedCategory);
            await db.SaveChangesAsync();
        });

        try
        {
            await SeedPoisAsync(userId, new Poi { CategoryId = deletedCategory.Id, Geometry = Pt(29.01, 41.01) });

            var response = await LocationAnalysisAsync(client, (1, 50), (5, 50));

            Assert.Empty(await HeatmapAsync(response));
        }
        finally
        {
            await WithDbAsync(async db =>
            {
                db.Pois.RemoveRange(db.Pois.Where(p => p.CategoryId == deletedCategory.Id));
                db.PoiCategories.Remove(await db.PoiCategories.FindAsync(deletedCategory.Id) ?? deletedCategory);
                await db.SaveChangesAsync();
            });
        }
    }

    [Fact]
    public async Task LocationAnalysis_ScoresNotSummingTo100_Returns400()
    {
        var (_, client) = await CreateUserAsync([], ["location_analysis"]);

        var response = await LocationAnalysisAsync(client, (1, 60), (5, 30));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
