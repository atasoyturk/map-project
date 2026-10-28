using System.Net;
using System.Net.Http.Json;
using BackendApi.DTOs.Poi;
using BackendApi.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Tests;

public sealed class PoiCategoryTests : ApiTestBase
{
    public PoiCategoryTests(ApiFixture fixture) : base(fixture) { }

    // Seed rows are inserted with explicit IDs; the identity sequence must still continue past them.
    [Fact]
    public async Task Create_AfterSeed_GetsIdAboveSeededIds()
    {
        var maxSeededId = 0;
        await WithDbAsync(async db => maxSeededId = await db.PoiCategories.MaxAsync(c => c.Id));

        var (_, client) = await CreateUserAsync([Roles.Admin]);
        var response = await client.PostAsJsonAsync("/api/poi-category", new { name = "Yeni", parentCategoryId = (int?)null });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<PoiCategoryResponseDto>();
        Assert.True(created!.Id > maxSeededId);
    }

    [Fact]
    public async Task Create_WithIcon_IconIsReturnedInTree()
    {
        var (_, client) = await CreateUserAsync([Roles.Admin]);
        var response = await client.PostAsJsonAsync("/api/poi-category", new { name = "İkonlu", parentCategoryId = (int?)null, icon = "🧪" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<PoiCategoryResponseDto>();

        var tree = await client.GetFromJsonAsync<List<PoiCategoryTreeDto>>("/api/poi-category/tree");

        Assert.Equal("🧪", tree!.Single(c => c.Id == created!.Id).Icon);
    }

    [Fact]
    public async Task Tree_SeededTopLevelCategories_HaveIcons()
    {
        var (_, client) = await CreateUserAsync([Roles.Admin]);

        var tree = await client.GetFromJsonAsync<List<PoiCategoryTreeDto>>("/api/poi-category/tree");

        Assert.All(tree!.Where(c => c.Id <= SeededCategoryCount), c => Assert.False(string.IsNullOrEmpty(c.Icon)));
    }

    private const int SeededCategoryCount = 34;
}
