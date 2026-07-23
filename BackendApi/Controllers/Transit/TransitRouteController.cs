using BackendApi.Authorization;
using BackendApi.DTOs.Transit;
using BackendApi.Services.Transit;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Transit;

[Route("api/transit-route")]
public sealed class TransitRouteController : ApiControllerBase
{
    private readonly ITransitRouteService _service;

    public TransitRouteController(ITransitRouteService service) => _service = service;

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

}