namespace BackendApi.DTOs.Geo;

public sealed record GeoPermissionResponseDto(
    int    Id,
    string Name,
    string WktGeometry,
    bool   IsActive
);