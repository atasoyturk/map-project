using BackendApi.DTOs.Geo;

namespace BackendApi.Services.Geo;

public interface IPointService
{
    Task<PointResponseDto>                SaveAsync(GeoRequestDto request, int userId, int? teamId, IEnumerable<string> roles);
    Task<PointResponseDto?>               UpdateAsync(int id, GeoRequestDto request, int userId, IEnumerable<string> roles);
    Task<IEnumerable<PointResponseDto>>   GetAllAsync(int userId, int? teamId, GeoViewMode viewMode);
    Task<bool>                            DeleteAsync(int id, int userId);
    Task<PointResponseDto?>               GetByIdAsync(int id, int userId);
}