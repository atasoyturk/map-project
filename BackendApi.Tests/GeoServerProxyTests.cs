using System.Net;
using BackendApi.Tests.Infrastructure;

namespace BackendApi.Tests;

public sealed class GeoServerProxyTests : ApiTestBase
{
    public GeoServerProxyTests(ApiFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Wfs_UnknownLayer_ReturnsBadRequest()
    {
        var (_, client) = await CreateUserAsync([Roles.BasicUser]);

        var response = await client.GetAsync("/api/proxy/geoserver/wfs?typeName=bad");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
