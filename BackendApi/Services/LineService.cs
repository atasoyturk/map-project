using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Entities;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services;

public sealed class LineService : ILineService
{
    private readonly AppDbContext _context;

    public LineService(AppDbContext context) => _context = context;

    public async Task<LineResponseDto> SaveAsync(GeoRequestDto request, int userId)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        var entity = new LineEntity
        {
            Geometry = geometry,
            Name     = request.Name,
            Color    = request.Color,
            UserId   = userId          // ← eklendi
        };

        _context.Lines.Add(entity);
        await _context.SaveChangesAsync();

        return new LineResponseDto(
            entity.Id,
            entity.Name,
            entity.Color,
            GeometryConverter.ToWkt(geometry)
        );
    }

    public async Task<IEnumerable<LineResponseDto>> GetAllAsync(int userId) =>
        await _context.Lines
            .Where(l => l.UserId == userId)   // ← filtreleme eklendi
            .Select(l => new LineResponseDto(
                l.Id,
                l.Name,
                l.Color,
                GeometryConverter.ToWkt(l.Geometry)))
            .ToListAsync();
}