using BackendApi.DTOs.Transit;

namespace BackendApi.Services.Transit;

public interface ITransitStopService
{
    Task<TransitStopResponseDto>              CreateAsync(TransitStopRequestDto request, int userId, IEnumerable<string> roles, bool hasAdminAccess);
    Task<TransitStopResponseDto?>             UpdateAsync(int id, TransitStopRequestDto request, int userId, IEnumerable<string> roles, bool hasAdminAccess);
    Task<bool>                                DeleteAsync(int id, int userId, bool hasAdminAccess);
    Task<IEnumerable<TransitStopResponseDto>> GetAllAsync();
    Task<TransitStopResponseDto?>             GetByIdAsync(int id);
}