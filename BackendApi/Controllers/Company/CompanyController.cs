// BackendApi/Controllers/Company/CompanyController.cs
using BackendApi.Authorization;
using BackendApi.DTOs.Company;
using BackendApi.Services.Company;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Company;

[Route("api/company")]
public sealed class CompanyController : ApiControllerBase
{
    private readonly ICompanyService _service;

    public CompanyController(ICompanyService service) => _service = service;

    [HttpGet]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Create([FromBody] CompanyRequestDto request)
        => Created(string.Empty, await _service.CreateAsync(request));

    [HttpPut("{id:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Update(int id, [FromBody] CompanyRequestDto request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.DeleteAsync(id, userId.Value);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}/routes")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> GetRoutes(int id) => Ok(await _service.GetRoutesByCompanyAsync(id));

    [HttpGet("by-route/{transitRouteId:int}")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> GetCompaniesByRoute(int transitRouteId)
        => Ok(await _service.GetCompaniesByRouteAsync(transitRouteId));

    [HttpPost("{id:int}/routes")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> AssignRoute(int id, [FromBody] AssignRouteToCompanyDto request)
    {
        await _service.AssignRouteAsync(id, request.TransitRouteId);
        return NoContent();
    }

    [HttpDelete("{id:int}/routes/{transitRouteId:int}")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> RemoveRoute(int id, int transitRouteId)
    {
        var result = await _service.RemoveRouteAsync(id, transitRouteId);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("stats")]
    [RequirePermission("admin_access")]
    public async Task<IActionResult> GetStats() => Ok(await _service.GetStatsAsync());

    [HttpGet("shipments")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> GetShipments([FromQuery] int? transitRouteId)
        => Ok(await _service.GetShipmentRecordsAsync(transitRouteId));

    [HttpGet("routes/unassigned")]
    [RequirePermission("transit_route_manage")]
    public async Task<IActionResult> GetUnassignedRoutes() => Ok(await _service.GetUnassignedRoutesAsync());
}