using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Entities;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<PointResponseDto>> GetAllAsync() =>
        await _context.Points
            .Select(p => new PointResponseDto(
                p.Id,
                p.Name,
                p.Color,
                GeometryConverter.ToWkt(p.Geometry)))
            .ToListAsync();
}