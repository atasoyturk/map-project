using BackendApi.DTOs.Transit;

namespace BackendApi.Services.Transit;

public interface ITripPlanningService
{
    Task<TripPlanResultDto?> PlanTripAsync(double startLon, double startLat, double endLon, double endLat);
}