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
            UserId   = userId
        };

        _context.Lines.Add(entity);
        await _context.SaveChangesAsync();

        return new LineResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(geometry));
    }

    public async Task<IEnumerable<LineResponseDto>> GetAllAsync(int userId) =>
        await _context.Lines
            .Where(l => l.UserId == userId && !l.IsDeleted)
            .Select(l => new LineResponseDto(l.Id, l.Name, l.Color,
                GeometryConverter.ToWkt(l.Geometry)))
            .ToListAsync();

    public async Task<LineResponseDto?> UpdateAsync(int id, GeoRequestDto request, int userId)
    {
        var entity = await _context.Lines
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId && !l.IsDeleted);

        if (entity is null) return null;

        entity.Geometry     = GeometryConverter.FromWkt(request.WktGeometry);
        entity.Name         = request.Name;
        entity.Color        = request.Color;
        entity.ModifiedDate = DateTime.UtcNow;
        entity.ModifiedUserId  = userId; 

        await _context.SaveChangesAsync();

        return new LineResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry));
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.Lines
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId && !l.IsDeleted);

        if (entity is null) return false;

        entity.IsDeleted    = true;
        entity.ModifiedDate = DateTime.UtcNow;
        entity.ModifiedUserId  = userId; 
        await _context.SaveChangesAsync();
        return true;
    }
}