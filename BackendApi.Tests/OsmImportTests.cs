using System.Net;
using System.Net.Http.Json;
using BackendApi.DTOs.Poi;
using BackendApi.Services.Poi;
using BackendApi.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendApi.Tests;

public sealed class OsmImportTests : ApiTestBase
{
    private static readonly object Bbox = new { minLon = 28.9, minLat = 40.9, maxLon = 29.1, maxLat = 41.1 };

    private readonly FakeOverpassClient _overpass;

    public OsmImportTests(ApiFixture fixture) : base(fixture)
        => _overpass = fixture.Factory.Services.GetRequiredService<FakeOverpassClient>();

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _overpass.Reset();
    }

    private static OsmElement Node(long id, params (string Key, string Value)[] tags)
        => new("node", id, 41.0, 29.0, tags.ToDictionary(t => t.Key, t => t.Value));

    private static async Task<OsmImportResultDto> ImportAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/poi/import-osm", Bbox);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OsmImportResultDto>())!;
    }

    [Fact]
    public async Task NonAdmin_IsForbidden()
    {
        var (_, client) = await CreateUserAsync([Roles.PoiOperator]);

        var response = await client.PostAsJsonAsync("/api/poi/import-osm", Bbox);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Null(_overpass.LastQuery);
    }

    [Theory]
    [InlineData(28.0, 40.0, 29.0, 40.4)] // 1° wide
    [InlineData(29.0, 41.0, 28.9, 41.1)] // min > max
    public async Task InvalidBbox_Returns400WithoutCallingOverpass(double minLon, double minLat, double maxLon, double maxLat)
    {
        var (_, admin) = await CreateUserAsync([Roles.Admin]);

        var response = await admin.PostAsJsonAsync("/api/poi/import-osm", new { minLon, minLat, maxLon, maxLat });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Null(_overpass.LastQuery);
    }

    [Fact]
    public async Task OverpassFailure_Returns502WithTurkishMessage()
    {
        var (_, admin) = await CreateUserAsync([Roles.Admin]);
        _overpass.Failure = new HttpRequestException("Overpass query incomplete: runtime error: Query timed out");

        var response = await admin.PostAsJsonAsync("/api/poi/import-osm", Bbox);

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        Assert.Contains("OSM verisi alınamadı", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Import_MapsTagsToCategoriesAndSkipsUnmappedOrUnnamed()
    {
        var (adminId, admin) = await CreateUserAsync([Roles.Admin]);
        _overpass.Elements =
        [
            Node(1, ("amenity", "restaurant"), ("name", "Lokanta"), ("opening_hours", "Mo-Su 09:00-22:00")),
            new OsmElement("way", 2, 41.01, 29.01, new Dictionary<string, string> { ["shop"] = "mall", ["name"] = "AVM" }),
            Node(3, ("amenity", "place_of_worship"), ("religion", "muslim"), ("name", "Cami")),
            Node(4, ("amenity", "place_of_worship"), ("religion", "christian"), ("name", "Kilise")),
            Node(5, ("amenity", "cafe")),
            Node(6, ("highway", "bus_stop"), ("name", "Durak")),
            Node(7, ("amenity", "pharmacy"), ("name", "Eczane"), ("opening_hours", new string('x', 201))),
        ];

        var result = await ImportAsync(admin);

        Assert.Equal((4, 0, 0), (result.Added, result.Updated, result.Skipped));
        Assert.Empty(result.SkippedCategories);
        Assert.Contains("[timeout:", _overpass.LastQuery);
        Assert.Contains("[maxsize:", _overpass.LastQuery);
        Assert.Contains("out center", _overpass.LastQuery);
        Assert.Contains("(40.9,28.9,41.1,29.1)", _overpass.LastQuery);

        await WithDbAsync(async db =>
        {
            var pois = await db.Pois.ToDictionaryAsync(p => p.OsmId!);
            Assert.Equal(["node/1", "node/3", "node/7", "way/2"], pois.Keys.Order());
            Assert.Equal(2,  pois["node/1"].CategoryId);
            Assert.Equal("Mo-Su 09:00-22:00", pois["node/1"].WorkingHours);
            Assert.Equal(18, pois["way/2"].CategoryId);
            Assert.Equal(29.01, pois["way/2"].Geometry.X);
            Assert.Equal(32, pois["node/3"].CategoryId);
            Assert.Equal(string.Empty, pois["node/7"].WorkingHours);
            Assert.All(pois.Values, p =>
            {
                Assert.Equal(adminId, p.UserId);
                Assert.Null(p.ModifiedUserId);
            });
        });
    }

    [Fact]
    public async Task ReImport_UpdatesUntouchedRows_SkipsUserEditedAndDeletedRows()
    {
        var (_, admin) = await CreateUserAsync([Roles.Admin]);
        _overpass.Elements =
        [
            Node(1, ("amenity", "cafe"), ("name", "Kafe A")),
            Node(2, ("amenity", "cafe"), ("name", "Kafe B")),
            Node(3, ("amenity", "cafe"), ("name", "Kafe C")),
        ];
        await ImportAsync(admin);

        var ids = new Dictionary<string, int>();
        await WithDbAsync(async db =>
        {
            foreach (var p in await db.Pois.ToListAsync()) ids[p.OsmId!] = p.Id;
        });

        var edit = await admin.PutAsJsonAsync($"/api/poi/{ids["node/2"]}",
            new { name = "Kullanıcı adı", workingHours = "09-18", wktGeometry = "POINT(29 41)", categoryId = 3 });
        edit.EnsureSuccessStatusCode();
        (await admin.DeleteAsync($"/api/poi/{ids["node/3"]}")).EnsureSuccessStatusCode();

        _overpass.Elements =
        [
            Node(1, ("amenity", "cafe"), ("name", "Kafe A yeni")),
            Node(2, ("amenity", "cafe"), ("name", "Kafe B yeni")),
            Node(3, ("amenity", "cafe"), ("name", "Kafe C yeni")),
            Node(4, ("amenity", "cafe"), ("name", "Kafe D")),
        ];
        var result = await ImportAsync(admin);

        Assert.Equal((1, 1, 2), (result.Added, result.Updated, result.Skipped));
        await WithDbAsync(async db =>
        {
            var pois = await db.Pois.ToDictionaryAsync(p => p.OsmId!);
            Assert.Equal(4, pois.Count);
            Assert.Equal("Kafe A yeni", pois["node/1"].Name);
            Assert.Null(pois["node/1"].ModifiedUserId);
            Assert.Equal("Kullanıcı adı", pois["node/2"].Name);
            Assert.Equal("Kafe C", pois["node/3"].Name);
            Assert.True(pois["node/3"].IsDeleted);
            Assert.Equal("Kafe D", pois["node/4"].Name);
        });
    }

    [Fact]
    public async Task DeletedCategory_IsSkippedAndReported()
    {
        var (_, admin) = await CreateUserAsync([Roles.Admin]);
        _overpass.Elements =
        [
            Node(1, ("amenity", "cafe"),       ("name", "Kafe")),
            Node(2, ("amenity", "restaurant"), ("name", "Lokanta")),
        ];

        // Seed tables survive Respawn, so the category must be restored afterwards.
        await SetCategoryDeletedAsync(3, true);
        try
        {
            var result = await ImportAsync(admin);

            Assert.Equal([3], result.SkippedCategories);
            Assert.Equal(1, result.Added);
            Assert.DoesNotContain("cafe", _overpass.LastQuery);
            await WithDbAsync(async db => Assert.Equal("node/2", (await db.Pois.SingleAsync()).OsmId));
        }
        finally
        {
            await SetCategoryDeletedAsync(3, false);
        }
    }

    private Task SetCategoryDeletedAsync(int categoryId, bool isDeleted) => WithDbAsync(async db =>
    {
        var category = await db.PoiCategories.SingleAsync(c => c.Id == categoryId);
        category.IsDeleted = isDeleted;
        await db.SaveChangesAsync();
    });
}
