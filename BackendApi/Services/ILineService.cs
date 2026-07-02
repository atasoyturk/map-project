using BackendApi.DTOs;

namespace BackendApi.Services;

public interface ILineService
{
    Task<LineResponseDto>             SaveAsync(GeoRequestDto request, int userId);
    Task<IEnumerable<LineResponseDto>> GetAllAsync(int userId);
}