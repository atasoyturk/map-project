namespace BackendApi.DTOs;

public sealed record SearchResponseDto(
    IEnumerable<PointResponseDto>   Points,
    IEnumerable<LineResponseDto>    Lines,
    IEnumerable<PolygonResponseDto> Polygons
);