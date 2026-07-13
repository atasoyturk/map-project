namespace BackendApi.DTOs.Annotation;

public sealed record AnnotationResponseDto(
    int Id, string NoteText, string WktGeometry,
    int UserId, int? TeamId, DateTime CreatedDate);