using System.Text;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BackendApi.Services;
using BackendApi.Settings;
using BackendApi.Data;
using BackendApi.Repositories;

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
              .WithHeaders("Authorization", "Content-Type")));

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

builder.Services.AddAuthorization();

// Application Services 
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IPointService,   PointService>();
builder.Services.AddScoped<ILineService,    LineService>();
builder.Services.AddScoped<IPolygonService, PolygonService>();

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