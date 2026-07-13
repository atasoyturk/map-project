using BackendApi.DTOs.Geo;

namespace BackendApi.Services.Geo;

public interface IPolygonService
{
    Task<PolygonResponseDto>                SaveAsync(GeoRequestDto request, int userId, int? teamId, IEnumerable<string> roles);
    Task<PolygonResponseDto?>               UpdateAsync(int id, GeoRequestDto request, int userId, IEnumerable<string> roles);
    Task<IEnumerable<PolygonResponseDto>>   GetAllAsync(int userId, int? teamId, GeoViewMode viewMode);
    Task<bool>                              DeleteAsync(int id, int userId);
    Task<PolygonResponseDto?>               GetByIdAsync(int id, int userId);
}