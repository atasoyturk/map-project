using BackendApi.Authorization;
using BackendApi.DTOs.Poi;
using BackendApi.Services.Poi;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Poi;

[Route("api/poi-category")]
public sealed class PoiCategoryController : ApiControllerBase
{
    private readonly IPoiCategoryService _service;

    public PoiCategoryController(IPoiCategoryService service) => _service = service;

    [HttpGet("tree")]
    [RequirePermission("poi_read")]
    public async Task<IActionResult> GetTree() => Ok(await _service.GetTreeAsync());

    [HttpPost]
    [RequirePermission("poi_category_manage")]
    public async Task<IActionResult> Create([FromBody] PoiCategoryRequestDto request)
        => Created(string.Empty, await _service.CreateAsync(request));

    [HttpPut("{id:int}")]
    [RequirePermission("poi_category_manage")]
    public async Task<IActionResult> Update(int id, [FromBody] PoiCategoryRequestDto request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("poi_category_manage")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}