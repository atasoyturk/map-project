using BackendApi.Authorization;
using BackendApi.DTOs.Poi;
using BackendApi.Services.Poi;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[Route("api/poi-category")]
public sealed class PoiCategoryController : ApiControllerBase
{
    private readonly IPoiCategoryService _service;
    private readonly ILogger<PoiCategoryController> _logger;

    public PoiCategoryController(IPoiCategoryService service, ILogger<PoiCategoryController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    [HttpGet("tree")]
    [RequirePermission("poi_read")]
    public async Task<IActionResult> GetTree()
    {
        try { return Ok(await _service.GetTreeAsync()); }
        catch (Exception ex) { _logger.LogError(ex, "GetTree failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPost]
    [RequirePermission("poi_category_manage")]
    public async Task<IActionResult> Create([FromBody] PoiCategoryRequestDto request)
    {
        try { return Created(string.Empty, await _service.CreateAsync(request)); }
        catch (Exception ex) { _logger.LogError(ex, "CreateCategory failed"); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpPut("{id:int}")]
    [RequirePermission("poi_category_manage")]
    public async Task<IActionResult> Update(int id, [FromBody] PoiCategoryRequestDto request)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "UpdateCategory failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("poi_category_manage")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
        
        catch (InvalidOperationException ex) { return Conflict(ex.Message); } 
        catch (Exception ex) { _logger.LogError(ex, "DeleteCategory failed for id {Id}", id); return StatusCode(500, "Sunucu hatası."); }
    }
}