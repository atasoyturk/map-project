using BackendApi.Authorization;
using BackendApi.DTOs.Transit;
using BackendApi.Services.Transit;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Transit;

[Route("api/trip-planning")]
public sealed class TripPlanningController : ApiControllerBase
{
    private readonly ITripPlanningService _service;

    public TripPlanningController(ITripPlanningService service) => _service = service;

    [HttpPost("plan")]
    [RequirePermission("transit_stop_read")]
    public async Task<IActionResult> Plan([FromBody] TripPlanRequestDto request)
    {
        var result = await _service.PlanTripAsync(request.StartLon, request.StartLat, request.EndLon, request.EndLat);
        return result is null ? NotFound("Bu iki nokta arasında uygun bir güzergah bulunamadı.") : Ok(result);
    }
}