using BackendApi.DTOs.Geo;

namespace BackendApi.Services.Geo;

public interface ILineService
{
    Task<LineResponseDto> SaveAsync(GeoRequestDto request, int userId, int? teamId, IEnumerable<string> roles);
    Task<LineResponseDto?> UpdateAsync(int id, GeoRequestDto request, int userId, IEnumerable<string> roles);
    Task<bool> DeleteAsync(int id, int userId, IEnumerable<string> roles); 
    Task<IEnumerable<LineResponseDto>> GetAllAsync(int userId, int? teamId, GeoViewMode viewMode);
    Task<LineResponseDto?> GetByIdAsync(int id, int userId, IEnumerable<string> roles); 
}
