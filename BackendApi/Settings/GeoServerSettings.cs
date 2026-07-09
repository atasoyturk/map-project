namespace BackendApi.Settings;

public sealed class GeoServerSettings
{
    public string BaseUrl   { get; init; } = string.Empty;
    public string Workspace { get; init; } = string.Empty;
    public string Username  { get; init; } = string.Empty;
    public string Password  { get; init; } = string.Empty;
}