using BackendApi.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendApi.Tests;

public sealed class OverpassHttpClientTests : ApiTestBase
{
    private readonly ApiFixture _fixture;

    public OverpassHttpClientTests(ApiFixture fixture) : base(fixture) => _fixture = fixture;

    // A value different from appsettings.json proves the User-Agent comes from config, not a hard-coded string.
    [Fact]
    public async Task OverpassClient_SendsConfiguredUserAgent()
    {
        await using var factory = _fixture.Factory.WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OverpassSettings:UserAgent"] = "Test/9.9",
            })));

        var client = factory.Services.GetRequiredService<IHttpClientFactory>().CreateClient("Overpass");

        Assert.Equal("Test/9.9", client.DefaultRequestHeaders.UserAgent.ToString());
    }
}
