using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Entities;
using BackendApi.Helpers;

namespace BackendApi.Services;

public sealed class LineService : ILineService
{
    private readonly AppDbContext _context;

    public LineService(AppDbContext context) => _context = context;

    public async Task<LineResponseDto> SaveAsync(GeoRequestDto request)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        var entity = new LineEntity
        {
            Geometry = geometry,
            Name     = request.Name,
            Color    = request.Color
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
}