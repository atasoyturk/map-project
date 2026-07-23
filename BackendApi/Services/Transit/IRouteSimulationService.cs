using BackendApi.DTOs.Transit;

namespace BackendApi.Services.Transit;

public interface IRouteSimulationService
{
    Task<SimulationActionResponseDto> StartAsync(int routeId, int[] vehicleIds, int startedByUserId);
    Task<SimulationActionResponseDto> StopAsync(int routeId, int[] vehicleIds);
    IEnumerable<VehiclePositionDto>   GetStatus(int routeId);
}