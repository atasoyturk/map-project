using BackendApi.DTOs;

namespace BackendApi.Services.Geo;

public interface IPointService
{
    Task<PointResponseDto>                SaveAsync(GeoRequestDto request, int userId, IEnumerable<string> roles);
    Task<PointResponseDto?>               UpdateAsync(int id, GeoRequestDto request, int userId, IEnumerable<string> roles);
    Task<IEnumerable<PointResponseDto>>   GetAllAsync(int userId);
    Task<bool>                            DeleteAsync(int id, int userId);
    Task<PointResponseDto?>               GetByIdAsync(int id, int userId);
    
}