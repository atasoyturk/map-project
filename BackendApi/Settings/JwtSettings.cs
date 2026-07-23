using System.ComponentModel.DataAnnotations;

namespace BackendApi.Settings;
//POCO 
public sealed class JwtSettings
{   
    [Required(AllowEmptyStrings = false, ErrorMessage = "JwtSettings:SecretKey yapılandırması zorunludur.")]
    public string SecretKey { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "JwtSettings:Issuer yapılandırması zorunludur.")]
    public string Issuer    { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "JwtSettings:Audience yapılandırması zorunludur.")]
    public string Audience  { get; init; } = string.Empty;

    [Range(1, 1440, ErrorMessage = "JwtSettings:ExpiresInMinutes 1 ile 1440 (24 saat) arasında olmalıdır.")]
    public int    ExpiresInMinutes { get; init; } = 5;

    [Range(1, int.MaxValue, ErrorMessage = "JwtSettings:ExtendedExpiresInMinutes geçerli bir değer olmalıdır.")]
    public int    ExtendedExpiresInMinutes { get; init; } = 5_256_000; // ~10 yıl
}