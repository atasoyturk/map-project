using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs.Poi;

public sealed record PoiCategoryRequestDto(
    [Required, MaxLength(100)] string Name,
    int? ParentCategoryId,
    [MaxLength(16)] string? Icon = null);

public sealed record PoiCategoryResponseDto(
    int     Id,
    string  Name,
    int?    ParentCategoryId,
    string? Icon);

public sealed record PoiCategoryTreeDto(
    int     Id,
    string  Name,
    string? Icon,
    IList<PoiCategoryTreeDto> Children);