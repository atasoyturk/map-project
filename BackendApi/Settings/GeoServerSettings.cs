using System.ComponentModel.DataAnnotations;

namespace BackendApi.Settings;

public sealed class GeoServerSettings
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "GeoServerSettings:BaseUrl yapılandırması zorunludur.")]
    [Url(ErrorMessage = "GeoServerSettings:BaseUrl geçerli bir URL olmalıdır.")]
    public string BaseUrl   { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "GeoServerSettings:Workspace yapılandırması zorunludur.")]
    public string Workspace { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "GeoServerSettings:Username yapılandırması zorunludur.")]
    public string Username  { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "GeoServerSettings:Password yapılandırması zorunludur.")]
    public string Password  { get; init; } = string.Empty;
}
