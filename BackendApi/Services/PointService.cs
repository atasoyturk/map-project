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

    public async Task<PointResponseDto> SaveAsync(GeoRequestDto request, int userId)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        var entity = new PointEntity
        {
            Geometry = geometry,
            Name     = request.Name,
            Color    = request.Color,
            UserId   = userId
        };

        _context.Points.Add(entity);
        await _context.SaveChangesAsync();

        return new PointResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(geometry));
    }

    public async Task<IEnumerable<PointResponseDto>> GetAllAsync(int userId) =>
        await _context.Points
            .Where(p => p.UserId == userId && !p.IsDeleted)  
            .Select(p => new PointResponseDto(p.Id, p.Name, p.Color,
                GeometryConverter.ToWkt(p.Geometry)))
            .ToListAsync();

    public async Task<PointResponseDto?> UpdateAsync(int id, GeoRequestDto request, int userId)
    {
        var entity = await _context.Points
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return null;

        entity.Geometry     = GeometryConverter.FromWkt(request.WktGeometry);
        entity.Name         = request.Name;
        entity.Color        = request.Color;
        entity.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new PointResponseDto(entity.Id, entity.Name, entity.Color,
            GeometryConverter.ToWkt(entity.Geometry));
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.Points
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

        if (entity is null) return false;

        entity.IsDeleted    = true;
        entity.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}