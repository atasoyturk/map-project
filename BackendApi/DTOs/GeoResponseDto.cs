namespace BackendApi.DTOs;

public sealed record PointResponseDto(
    int    Id, 
    string Name, 
    string Color, 
    string WktGeometry);

public sealed record LineResponseDto(
    int    Id, 
    string Name, 
    string Color, 
    string WktGeometry);

public sealed record PolygonResponseDto(
    int    Id,
    string Name,
    string Color,
    string WktGeometry,
    int    IntersectedInventoryCount   
);