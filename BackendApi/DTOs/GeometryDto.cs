namespace BackendApi.DTOs;

public sealed record PointDto(int Id, string WktGeometry);
public sealed record LineDto(int Id, string WktGeometry);
public sealed record PolygonDto(int Id, string WktGeometry);