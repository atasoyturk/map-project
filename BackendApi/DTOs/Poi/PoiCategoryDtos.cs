using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Poi;

public sealed record PoiCategoryRequestDto(
    [Required, MaxLength(100)] string Name,
    int? ParentCategoryId);

public sealed record PoiCategoryResponseDto(
    int    Id,
    string Name,
    int?   ParentCategoryId);

public sealed record PoiCategoryTreeDto(
    int    Id,
    string Name,
    IList<PoiCategoryTreeDto> Children);