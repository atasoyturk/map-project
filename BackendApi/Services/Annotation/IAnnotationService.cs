using BackendApi.DTOs.Annotation;

namespace BackendApi.Services.Annotation;

public interface IAnnotationService
{
    Task<AnnotationResponseDto> SaveAsync(AnnotationRequestDto request, int userId, int? teamId, IEnumerable<string> roles);
    Task<IEnumerable<AnnotationResponseDto>> GetAllAsync(int userId, int? teamId, bool isAdmin);
    Task<bool> DeleteAsync(int id, int userId, IEnumerable<string> roles);

}