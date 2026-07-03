using BackendApi.DTOs;

namespace BackendApi.Services;

public interface IPolygonService
{
    Task<PolygonResponseDto>             SaveAsync(GeoRequestDto request, int userId);
    Task<IEnumerable<PolygonResponseDto>> GetAllAsync(int userId);
    Task<PolygonResponseDto?>             UpdateAsync(int id, GeoRequestDto request, int userId);
    Task<bool>                          DeleteAsync(int id, int userId);
}