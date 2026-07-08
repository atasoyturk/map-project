namespace BackendApi.DTOs.Geo;

public sealed record GeoPermissionResponseDto(
    int     Id,
    int?    UserId,
    int?    RoleId,
    string  WktGeometry,
    bool    IsActive
);