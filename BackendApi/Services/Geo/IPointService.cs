using BackendApi.DTOs;

namespace BackendApi.Services;

public interface IPointService
{
    Task<PointResponseDto>                SaveAsync(GeoRequestDto request, int userId);
    Task<IEnumerable<PointResponseDto>>   GetAllAsync(int userId);
    Task<PointResponseDto?>               UpdateAsync(int id, GeoRequestDto request, int userId);
    Task<bool>                            DeleteAsync(int id, int userId);
    Task<PointResponseDto?>               GetByIdAsync(int id, int userId);
}