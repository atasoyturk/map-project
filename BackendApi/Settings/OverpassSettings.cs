using System.ComponentModel.DataAnnotations;

namespace BackendApi.Settings;

public sealed class OverpassSettings
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "OverpassSettings:BaseUrl yapılandırması zorunludur.")]
    [Url(ErrorMessage = "OverpassSettings:BaseUrl geçerli bir URL olmalıdır.")]
    public string BaseUrl        { get; init; } = string.Empty;

    // Sent as the query's [timeout:...]; the HttpClient waits a little longer than this.
    [Range(10, 300, ErrorMessage = "OverpassSettings:TimeoutSeconds 10 ile 300 arasında olmalıdır.")]
    public int    TimeoutSeconds { get; init; } = 90;

    // The public Overpass server expects a User-Agent identifying the client.
    [Required(AllowEmptyStrings = false, ErrorMessage = "OverpassSettings:UserAgent yapılandırması zorunludur.")]
    public string UserAgent      { get; init; } = string.Empty;
}
