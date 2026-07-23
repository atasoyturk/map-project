using System.ComponentModel.DataAnnotations;

namespace BackendApi.Settings;

public sealed class OsrmSettings
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "OsrmSettings:BaseUrl yapılandırması zorunludur.")]
    [Url(ErrorMessage = "OsrmSettings:BaseUrl geçerli bir URL olmalıdır.")]
    public string BaseUrl { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "OsrmSettings:Profile yapılandırması zorunludur.")]
    public string Profile { get; init; } = "driving";
}