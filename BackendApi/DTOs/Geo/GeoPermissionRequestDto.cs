namespace BackendApi.DTOs.Geo;

public sealed class GeoPermissionRequestDto
{
    public string Name        { get; init; } = string.Empty;
    public string WktGeometry { get; init; } = string.Empty;
}