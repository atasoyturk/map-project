using BackendApi.DTOs;

namespace BackendApi.Services;

public interface IPolygonService
{
    Task<PolygonResponseDto> SaveAsync(GeoRequestDto request);
    Task<IEnumerable<PolygonResponseDto>> GetAllAsync();
}