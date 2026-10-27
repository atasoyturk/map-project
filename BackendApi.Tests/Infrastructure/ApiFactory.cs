using BackendApi.Data;
using BackendApi.Services.Transit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BackendApi.Tests.Infrastructure;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public ApiFactory(string connectionString) => _connectionString = connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Not "Development": keeps user-secrets (real connection string) out of the test host.
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = _connectionString,
            ["JwtSettings:SecretKey"]               = "test-only-secret-key-at-least-32-bytes-long!",
            ["GeoServerSettings:Username"]          = "test",
            ["GeoServerSettings:Password"]          = "test",
        }));

        builder.ConfigureTestServices(services =>
        {
            // Re-register the DbContext so it can only ever point at the test container.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_connectionString, o => o.UseNetTopologySuite()));

            services.RemoveAll<IOsrmService>();
            services.AddScoped<IOsrmService, FakeOsrmService>();
        });
    }
}
