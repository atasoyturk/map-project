namespace BackendApi.DTOs.Geo;

public sealed class GeoValidateRequestDto
{
    public string WktGeometry { get; init; } = string.Empty;
}