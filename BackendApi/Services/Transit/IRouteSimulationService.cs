using BackendApi.DTOs.Transit;

namespace BackendApi.Services.Transit;

public interface IRouteSimulationService
{
    Task<(bool Success, string? Error)> StartAsync(int routeId, int startedByUserId);
    Task<(bool Success, string? Error)> StopAsync(int routeId);
    VehiclePositionDto? GetStatus(int routeId);
}