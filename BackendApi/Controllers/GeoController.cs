using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Entities;
using BackendApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public sealed class GeoController : ControllerBase
{
    private readonly AppDbContext _context;

    public GeoController(AppDbContext context) => _context = context;

    [HttpPost("point")]
    public async Task<IActionResult> SavePoint([FromBody] GeoRequestDto request)
    {
        try
        {
            var geometry = GeometryConverter.FromWkt(request.WktGeometry);
            _context.Points.Add(new PointEntity { Geometry = geometry });
            await _context.SaveChangesAsync();
            return Created(string.Empty, null);
        }
        catch (Exception)
        {
            return BadRequest("Geçersiz WKT formatı.");
        }
    }

    [HttpPost("line")]
    public async Task<IActionResult> SaveLine([FromBody] GeoRequestDto request)
    {
        try
        {
            var geometry = GeometryConverter.FromWkt(request.WktGeometry);
            _context.Lines.Add(new LineEntity { Geometry = geometry });
            await _context.SaveChangesAsync();
            return Created(string.Empty, null);
        }
        catch (Exception)
        {
            return BadRequest("Geçersiz WKT formatı.");
        }
    }

    [HttpPost("polygon")]
    public async Task<IActionResult> SavePolygon([FromBody] GeoRequestDto request)
    {
        try
        {
            var geometry = GeometryConverter.FromWkt(request.WktGeometry);
            _context.Polygons.Add(new PolygonEntity { Geometry = geometry });
            await _context.SaveChangesAsync();
            return Created(string.Empty, null);
        }
        catch (Exception)
        {
            return BadRequest("Geçersiz WKT formatı.");
        }
    }
}