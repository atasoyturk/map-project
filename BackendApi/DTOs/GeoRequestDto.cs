using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs;

public sealed class GeoRequestDto
{
    [Required, MinLength(1)]
    public string WktGeometry { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
}