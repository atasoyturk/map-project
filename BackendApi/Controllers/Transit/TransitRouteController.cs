using BackendApi.Authorization;
using BackendApi.DTOs.Transit;
using BackendApi.Services.Transit;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Transit;

[Route("api/transit-route")]
public sealed class TransitRouteController : ApiControllerBase
{
    private readonly ITransitRouteService    _service;
    private readonly IRouteSimulationService _simulationService;

    public TransitRouteController(ITransitRouteService service, IRouteSimulationService simulationService)
    {
        _service           = service;
        _simulationService = simulationService;
    }

    [HttpGet]
    [RequirePermission("transit_stop_read")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    [RequirePermission("transit_stop_read")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await _service.GetDetailAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> Create([FromBody] TransitRouteRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        return Created(string.Empty, await _service.CreateAsync(request, userId.Value));
    }

    [HttpPut("{id:int}")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> Update(int id, [FromBody] TransitRouteRequestDto request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.DeleteAsync(id, userId.Value);
        return result ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}/stops/reorder")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> ReorderStops(int id, [FromBody] ReorderStopsDto request)
    {
        var result = await _service.ReorderStopsAsync(id, request.StopIdsInOrder);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/generate-route")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> GenerateRoute(int id)
    {
        var result = await _service.GenerateRouteAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}/route-geometry")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> ClearRouteGeometry(int id)
    {
        var result = await _service.ClearRouteGeometryAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:int}/simulation/start")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> StartSimulation(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var (success, error) = await _simulationService.StartAsync(id, userId.Value);
        return success ? NoContent() : Conflict(error);
    }

    [HttpPost("{id:int}/simulation/stop")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> StopSimulation(int id)
    {
        var (success, error) = await _simulationService.StopAsync(id);
        return success ? NoContent() : NotFound(error);
    }

    [HttpGet("{id:int}/simulation/status")]
    public IActionResult GetSimulationStatus(int id)
    {
        var status = _simulationService.GetStatus(id);
        return status is null ? NoContent() : Ok(status);
    }
}