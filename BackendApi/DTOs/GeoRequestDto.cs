using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BackendApi.DTOs;

public sealed class GeoRequestDto
{
    [Required, MinLength(1)]
    public string WktGeometry { get; init; } = string.Empty;

    [MaxLength(100)]
    [RegularExpression(@"^[^<>""';&]*$",
        ErrorMessage = "İsim geçersiz karakter içeriyor.")]
    public string Name { get; init; } = string.Empty;

    [RegularExpression(@"^#[0-9A-Fa-f]{6}$",
        ErrorMessage = "Renk kodu geçerli bir hex renk kodu olmalıdır (#RRGGBB).")]
    public string Color { get; init; } = "#000000";
}