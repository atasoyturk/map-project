namespace BackendApi.DTOs.Geo;

public sealed class GeoPermissionRequestDto
{
    public int?   UserId          { get; init; }
    public int?   RoleId          { get; init; }
    public string WktGeometry     { get; init; } = string.Empty;
}