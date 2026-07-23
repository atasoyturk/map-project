using BackendApi.Authorization;
using BackendApi.DTOs.Company;
using BackendApi.Services.Company;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Company;

[Route("api/company-category")]
public sealed class CompanyCategoryController : ApiControllerBase
{
    private readonly ICompanyCategoryService _service;

    public CompanyCategoryController(ICompanyCategoryService service) => _service = service;

    [HttpGet]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Create([FromBody] CompanyCategoryRequestDto request)
        => Created(string.Empty, await _service.CreateAsync(request));

    [HttpDelete("{id:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.DeleteAsync(id, userId.Value);
        return result ? NoContent() : NotFound();
    }
}