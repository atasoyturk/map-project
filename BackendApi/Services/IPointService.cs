using BackendApi.DTOs;

namespace BackendApi.Services;

public interface IPointService
{
    Task<PointResponseDto> SaveAsync(GeoRequestDto request);
    Task<IEnumerable<PointResponseDto>> GetAllAsync(); 
}