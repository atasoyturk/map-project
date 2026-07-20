using BackendApi.DTOs.Transit;

namespace BackendApi.Services.Transit;

public interface ITransitRouteService
{
    Task<TransitRouteResponseDto>       CreateAsync(TransitRouteRequestDto request, int userId);
    Task<TransitRouteResponseDto?>      UpdateAsync(int id, TransitRouteRequestDto request);
    Task<bool>                          DeleteAsync(int id, int userId);
    Task<IEnumerable<TransitRouteResponseDto>> GetAllAsync();
    Task<TransitRouteDetailDto?>        GetDetailAsync(int id);
    Task<bool>                          ReorderStopsAsync(int routeId, int[] stopIdsInOrder);
}