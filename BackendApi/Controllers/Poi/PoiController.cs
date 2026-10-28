using BackendApi.Authorization;
using BackendApi.DTOs.Poi;
using BackendApi.Services.Poi;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Poi;

[Route("api/poi")]
public sealed class PoiController : ApiControllerBase
{
    private readonly IPoiService       _service;
    private readonly IOsmImportService _osmImportService;

    public PoiController(IPoiService service, IOsmImportService osmImportService)
    {
        _service          = service;
        _osmImportService = osmImportService;
    }

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

        var result = await _service.CreateAsync(request, userId.Value, GetUserRoles(), HasAdminAccess());
        return Created(string.Empty, result);
    }

    // One-off import for a bounded area; not meant to be called per request from the UI (Overpass usage limits).
    [HttpPost("import-osm")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> ImportOsm([FromBody] OsmImportRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var (result, error) = await _osmImportService.ImportAsync(request, userId.Value, cancellationToken);
        return error is null ? Ok(result) : StatusCode(502, error);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("poi_create")]
    public async Task<IActionResult> Update(int id, [FromBody] PoiRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.UpdateAsync(id, request, userId.Value, GetUserRoles(), HasAdminAccess());
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("poi_create")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.DeleteAsync(id, userId.Value, HasAdminAccess());
        return result ? NoContent() : NotFound();
    }
}