using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs;

public sealed class GeoRequestDto
{
    [Required, MinLength(1)]
    public string WktGeometry { get; init; } = string.Empty;
}