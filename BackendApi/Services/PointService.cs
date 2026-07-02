using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Entities;
using BackendApi.Helpers;

namespace BackendApi.Services;

public sealed class PointService : IPointService
{
    private readonly AppDbContext _context;

    public PointService(AppDbContext context) => _context = context;

    public async Task<PointResponseDto> SaveAsync(GeoRequestDto request)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        var entity = new PointEntity
        {
            Geometry = geometry,
            Name     = request.Name,
            Color    = request.Color
        };

        _context.Points.Add(entity);
        await _context.SaveChangesAsync();

        return new PointResponseDto(
            entity.Id,
            entity.Name,
            entity.Color,
            GeometryConverter.ToWkt(geometry)
        );
    }
}