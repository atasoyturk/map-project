using BackendApi.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BackendApi.Tests;

public sealed class OsrmHttpClientTests : ApiTestBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OsrmHttpClientTests(ApiFixture fixture) : base(fixture)
    {
        _httpClientFactory = fixture.Factory.Services.GetRequiredService<IHttpClientFactory>();
    }

    // The public OSRM server answers 403 to requests without a User-Agent.
    [Fact]
    public void OsrmClient_SendsConfiguredUserAgent()
    {
        var client = _httpClientFactory.CreateClient("Osrm");

        Assert.Equal("AtaGIS/1.0", client.DefaultRequestHeaders.UserAgent.ToString());
    }
}
