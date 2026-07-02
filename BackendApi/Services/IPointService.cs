using BackendApi.DTOs;

namespace BackendApi.Services;

public interface IPointService
{
    Task<PointResponseDto>             SaveAsync(GeoRequestDto request, int userId);
    Task<IEnumerable<PointResponseDto>> GetAllAsync(int userId);
}