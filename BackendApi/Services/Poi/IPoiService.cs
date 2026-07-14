using BackendApi.DTOs.Poi;

namespace BackendApi.Services.Poi;

public interface IPoiService
{
    Task<PoiResponseDto>              CreateAsync(PoiRequestDto request, int userId, IEnumerable<string> roles);
    Task<PoiResponseDto?>             UpdateAsync(int id, PoiRequestDto request, int userId, IEnumerable<string> roles);
    Task<bool>                        DeleteAsync(int id, int userId);
    Task<IEnumerable<PoiResponseDto>> GetAllAsync();
    Task<PoiResponseDto?>             GetByIdAsync(int id);
}