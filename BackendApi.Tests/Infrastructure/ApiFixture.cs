using BackendApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace BackendApi.Tests.Infrastructure;

// One PostGIS container + one API host for the whole test run.
public sealed class ApiFixture : IAsyncLifetime
{
    // Matches the local PostgreSQL 18 service.
    private const string PostgisImage = "postgis/postgis:18-3.6";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder(PostgisImage)
        .Build();

    private Respawner _respawner = null!;

    public ApiFactory Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        Factory = new ApiFactory(_container.GetConnectionString());

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        await using var connection = new NpgsqlConnection(_container.GetConnectionString());
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter        = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            // Seed data (HasData) and PostGIS metadata survive the reset.
            TablesToIgnore =
            [
                new Table("__EFMigrationsHistory"),
                new Table("spatial_ref_sys"),
                new Table("Roles"),
                new Table("Permissions"),
                new Table("RolePermissions"),
                new Table("tbl_poi_category"),
            ],
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(_container.GetConnectionString());
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public async Task DisposeAsync()
    {
        if (Factory is not null) await Factory.DisposeAsync();
        await _container.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ApiFixture>
{
    public const string Name = "Api";
}
