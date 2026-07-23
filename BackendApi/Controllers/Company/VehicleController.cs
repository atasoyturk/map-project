using BackendApi.Authorization;
using BackendApi.DTOs.Company;
using BackendApi.Services.Company;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Company;

[Route("api/vehicle")]
public sealed class VehicleController : ApiControllerBase
{
    private readonly IVehicleService _service;

    public VehicleController(IVehicleService service) => _service = service;

    [HttpGet]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("by-company/{companyId:int}")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> GetByCompany(int companyId) => Ok(await _service.GetByCompanyAsync(companyId));

    [HttpPost]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Create([FromBody] VehicleRequestDto request)
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