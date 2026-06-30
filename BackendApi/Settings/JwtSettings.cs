namespace BackendApi.Settings;
//POCO 
public sealed class JwtSettings
{   
    // all immutable 
    public string SecretKey { get; init; } = string.Empty;
    public string Issuer    { get; init; } = string.Empty;
    public string Audience  { get; init; } = string.Empty;
    public int    ExpiresInMinutes { get; init; } = 5;
}