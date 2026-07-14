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

using BackendApi.Services.Auth;
using BackendApi.Services.Geo;
using BackendApi.Services.Admin;
using BackendApi.Services.Analysis;
using BackendApi.Services.Permission;
using BackendApi.Services.Search;
using BackendApi.Services.Annotation;
using BackendApi.Services.Poi;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;

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
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer           = true,
            ValidIssuer              = jwtSettings.Issuer,
            ValidateAudience         = true,
            ValidAudience            = jwtSettings.Audience,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero   
        };
    });

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
});

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

// GeoServer Settings
builder.Services.Configure<GeoServerSettings>(
    builder.Configuration.GetSection("GeoServerSettings"));

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

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseNetTopologySuite()
    ));

builder.Services.AddScoped<IUserRepository, UserRepository>();

// Pipeline 
var app = builder.Build();

app.UseCors("ReactPolicy");      
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();