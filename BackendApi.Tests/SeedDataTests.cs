using BackendApi.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Tests;

public sealed class SeedDataTests : ApiTestBase
{
    public SeedDataTests(ApiFixture fixture) : base(fixture) { }

    // Seed values must be deterministic, otherwise every build differs from the migration snapshot.
    [Fact]
    public Task Model_HasNoPendingChanges() => WithDbAsync(db =>
    {
        Assert.False(db.Database.HasPendingModelChanges());
        return Task.CompletedTask;
    });
}
