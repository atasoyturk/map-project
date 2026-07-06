namespace BackendApi.DTOs;

public sealed record PointResponseDto(
    int      Id,
    string   Name,
    string   Color,
    string   WktGeometry,
    DateTime CreatedDate);   

public sealed record LineResponseDto(
    int      Id,
    string   Name,
    string   Color,
    string   WktGeometry,
    DateTime CreatedDate);   

public sealed record PolygonResponseDto(
    int      Id,
    string   Name,
    string   Color,
    string   WktGeometry,
    int      IntersectedInventoryCount,
    DateTime CreatedDate);   