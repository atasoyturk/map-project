// AnnotationRequestDto.cs
namespace BackendApi.DTOs.Annotation;

public sealed record AnnotationRequestDto(string NoteText, string WktGeometry);