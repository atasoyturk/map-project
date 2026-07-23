using System.Text;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using BackendApi.Settings;
using BackendApi.Data;
using BackendApi.Repositories;
using BackendApi.Authorization;
using BackendApi.Middleware;

using BackendApi.Services.Auth;
using BackendApi.Services.Geo;
using BackendApi.Services.Admin;
using BackendApi.Services.Analysis;
using BackendApi.Services.Permission;
using BackendApi.Services.Search;
using BackendApi.Services.Annotation;
using BackendApi.Services.Poi;
using BackendApi.Services.Transit;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Settings
builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("JwtSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// CORS policy
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
    options.AddPolicy("ReactPolicy", policy =>
        policy.WithOrigins(allowedOrigins)
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .WithHeaders("Authorization", "Content-Type")
              .SetPreflightMaxAge(TimeSpan.FromHours(1))));


// Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero
        };
    });

builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin_access",   policy =>
        policy.Requirements.Add(new PermissionRequirement("admin_access")));
    options.AddPolicy("point_create",   policy =>
        policy.Requirements.Add(new PermissionRequirement("point_create")));
    options.AddPolicy("line_create",    policy =>
        policy.Requirements.Add(new PermissionRequirement("line_create")));
    options.AddPolicy("polygon_create", policy =>
        policy.Requirements.Add(new PermissionRequirement("polygon_create")));
    options.AddPolicy("annotation_create", policy =>
        policy.Requirements.Add(new PermissionRequirement("annotation_create")));
    options.AddPolicy("annotation_read",   policy =>
        policy.Requirements.Add(new PermissionRequirement("annotation_read")));
    options.AddPolicy("poi_create", policy =>
        policy.Requirements.Add(new PermissionRequirement("poi_create")));
    options.AddPolicy("poi_read", policy =>
        policy.Requirements.Add(new PermissionRequirement("poi_read")));
    options.AddPolicy("poi_category_manage", policy =>
        policy.Requirements.Add(new PermissionRequirement("poi_category_manage")));
    options.AddPolicy("transit_stop_create", policy =>
        policy.Requirements.Add(new PermissionRequirement("transit_stop_create")));
    options.AddPolicy("transit_stop_read", policy =>
        policy.Requirements.Add(new PermissionRequirement("transit_stop_read")));
    options.AddPolicy("transit_route_manage", policy =>
        policy.Requirements.Add(new PermissionRequirement("transit_route_manage")));
    options.AddPolicy("area_scan", policy =>
        policy.Requirements.Add(new PermissionRequirement("area_scan")));
    options.AddPolicy("location_analysis", policy =>
        policy.Requirements.Add(new PermissionRequirement("location_analysis")));
    options.AddPolicy("heatmap_view", policy =>
        policy.Requirements.Add(new PermissionRequirement("heatmap_view")));
});

// Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Application Services 
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IPointService,   PointService>();
builder.Services.AddScoped<ILineService,    LineService>();
builder.Services.AddScoped<IPolygonService, PolygonService>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IPermissionService,    PermissionService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService,  UserService>();
builder.Services.AddScoped<IGeoPermissionService, GeoPermissionService>();
builder.Services.AddScoped<IGeoServerService, GeoServerService>();
builder.Services.AddScoped<IAnnotationService, AnnotationService>();
builder.Services.AddScoped<IPoiCategoryService, PoiCategoryService>();
builder.Services.AddScoped<IPoiService,         PoiService>();
builder.Services.AddScoped<ITransitRouteService, TransitRouteService>();
builder.Services.AddScoped<ITransitStopService,  TransitStopService>();

// GeoServer Settings
builder.Services
    .AddOptions<GeoServerSettings>()
    .Bind(builder.Configuration.GetSection("GeoServerSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// HttpClient — GeoServer proxy 
builder.Services.AddHttpClient("GeoServer", (serviceProvider, client) =>
{
    var settings = serviceProvider
        .GetRequiredService<IOptions<GeoServerSettings>>()
        .Value;

    var credentials = Convert.ToBase64String(
        Encoding.UTF8.GetBytes($"{settings.Username}:{settings.Password}"));
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
});

// OSRM Settings
builder.Services
    .AddOptions<OsrmSettings>()
    .Bind(builder.Configuration.GetSection("OsrmSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// HttpClient — OSRM routing service
builder.Services.AddHttpClient("Osrm");

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseNetTopologySuite()
    ));

builder.Services.AddScoped<IUserRepository, UserRepository>();

// Pipeline 
var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("ReactPolicy");      
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();