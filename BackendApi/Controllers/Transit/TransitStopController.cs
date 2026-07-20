using BackendApi.Authorization;
using BackendApi.DTOs.Transit;
using BackendApi.Services.Transit;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Transit;

[Route("api/transit-stop")]
public sealed class TransitStopController : ApiControllerBase
{
    private readonly ITransitStopService _service;

    public TransitStopController(ITransitStopService service) => _service = service;

    [HttpGet]
    [RequirePermission("transit_stop_read")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    [RequirePermission("transit_stop_read")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [RequirePermission("transit_stop_create")]
    public async Task<IActionResult> Create([FromBody] TransitStopRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.CreateAsync(request, userId.Value, GetUserRoles());
        return Created(string.Empty, result);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("transit_stop_create")]
    public async Task<IActionResult> Update(int id, [FromBody] TransitStopRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.UpdateAsync(id, request, userId.Value, GetUserRoles());
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("transit_stop_create")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.DeleteAsync(id, userId.Value);
        return result ? NoContent() : NotFound();
    }
}