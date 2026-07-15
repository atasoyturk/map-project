using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BackendApi.Settings;

namespace BackendApi.Authorization;

public sealed class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtSettings _jwtSettings;

    public ConfigureJwtBearerOptions(IOptions<JwtSettings> jwtSettings)
        => _jwtSettings = jwtSettings.Value;

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme) return;

        options.TokenValidationParameters.IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        options.TokenValidationParameters.ValidIssuer   = _jwtSettings.Issuer;
        options.TokenValidationParameters.ValidAudience = _jwtSettings.Audience;
    }

    public void Configure(JwtBearerOptions options) => Configure(null, options);
}