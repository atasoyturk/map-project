using BackendApi.DTOs;

namespace BackendApi.Services;

public interface ILineService
{
    Task<LineResponseDto>               SaveAsync(GeoRequestDto request, int userId, IEnumerable<string> roles);
    Task<IEnumerable<LineResponseDto>>  GetAllAsync(int userId);
    Task<LineResponseDto?>              UpdateAsync(int id, GeoRequestDto request, int userId, IEnumerable<string> roles);
    Task<bool>                          DeleteAsync(int id, int userId);
    Task<LineResponseDto?>              GetByIdAsync(int id, int userId);
}