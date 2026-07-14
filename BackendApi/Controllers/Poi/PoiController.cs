using BackendApi.Authorization;
using BackendApi.DTOs.Poi;
using BackendApi.Services.Poi;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Poi;

[Route("api/poi")]
public sealed class PoiController : ApiControllerBase
{
    private readonly IPoiService _service;
    private readonly ILogger<PoiController> _logger;

    public PoiController(IPoiService service, ILogger<PoiController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    [HttpGet]
    [RequirePermission("poi_read")]
    public async Task<IActionResult> GetAll()
    {
        try { return Ok(await _service.GetAllAsync()); }
        catch (Exception ex) { _logger.LogError(ex, "GetAllPois failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpGet("{id:int}")]
    [RequirePermission("poi_read")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex) { _logger.LogError(ex, "GetPoi failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost]
    [RequirePermission("poi_create")]
    public async Task<IActionResult> Create([FromBody] PoiRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var result = await _service.CreateAsync(request, userId.Value, GetUserRoles());
            return Created(string.Empty, result);
        }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
        catch (ArgumentException ex)            { return BadRequest(ex.Message); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreatePoi failed");
            return BadRequest("Geçersiz WKT formatı.");
        }
    }

    [HttpPut("{id:int}")]
    [RequirePermission("poi_create")]
    public async Task<IActionResult> Update(int id, [FromBody] PoiRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var result = await _service.UpdateAsync(id, request, userId.Value, GetUserRoles());
            return result is null ? NotFound() : Ok(result);
        }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
        catch (ArgumentException ex)            { return BadRequest(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "UpdatePoi failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("poi_create")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var result = await _service.DeleteAsync(id, userId.Value);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex) { _logger.LogError(ex, "DeletePoi failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }
}