using BackendApi.Authorization;
using BackendApi.DTOs.Poi;
using BackendApi.Services.Poi;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Poi;

[Route("api/poi")]
public sealed class PoiController : ApiControllerBase
{
    private readonly IPoiService _service;

    public PoiController(IPoiService service) => _service = service;

    [HttpGet]
    [RequirePermission("poi_read")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    [RequirePermission("poi_read")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [RequirePermission("poi_create")]
    public async Task<IActionResult> Create([FromBody] PoiRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.CreateAsync(request, userId.Value, GetUserRoles());
        return Created(string.Empty, result);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("poi_create")]
    public async Task<IActionResult> Update(int id, [FromBody] PoiRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.UpdateAsync(id, request, userId.Value, GetUserRoles());
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("poi_create")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.DeleteAsync(id, userId.Value);
        return result ? NoContent() : NotFound();
    }
}