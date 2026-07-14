using BackendApi.Data;
using BackendApi.DTOs.Poi;
using BackendApi.Entities.Poi;
using BackendApi.Helpers;
using BackendApi.Services.Geo;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PoiEntity = BackendApi.Entities.Poi.Poi;

namespace BackendApi.Services.Poi;

public sealed class PoiService : IPoiService
{
    private readonly AppDbContext          _context;
    private readonly IGeoPermissionService _geoPermissionService;

    public PoiService(AppDbContext context, IGeoPermissionService geoPermissionService)
    {
        _context              = context;
        _geoPermissionService = geoPermissionService;
    }

    public async Task<PoiResponseDto> CreateAsync(PoiRequestDto request, int userId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana POI ekleme yetkiniz bulunmamaktadır");

        if (!await _context.PoiCategories.AnyAsync(c => c.Id == request.CategoryId && !c.IsDeleted))
            throw new ArgumentException("Geçersiz kategori.");

        var entity = new PoiEntity
        {
            Name         = request.Name,
            WorkingHours = request.WorkingHours,
            Geometry     = (Point)geometry,   
            CategoryId   = request.CategoryId,
            UserId       = userId,
        };

        _context.Pois.Add(entity);
        await _context.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<PoiResponseDto?> UpdateAsync(int id, PoiRequestDto request, int userId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana POI ekleme yetkiniz bulunmamaktadır");

        if (!await _context.PoiCategories.AnyAsync(c => c.Id == request.CategoryId && !c.IsDeleted))
            throw new ArgumentException("Geçersiz kategori.");

        var entity = await _context.Pois
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);
        if (entity is null) return null;

        entity.Name           = request.Name;
        entity.WorkingHours   = request.WorkingHours;
        entity.Geometry       = (Point)geometry;
        entity.CategoryId     = request.CategoryId;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;

        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.Pois
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);
        if (entity is null) return false;

        entity.IsDeleted      = true;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;
        await _context.SaveChangesAsync();
        return true;
    }

    // Varsayım: POI paylaşılan/herkese açık veridir — Point/Line/Polygon'un aksine
    // sahiplik filtresi yok.
    public async Task<IEnumerable<PoiResponseDto>> GetAllAsync()
    {
        var entities = await _context.Pois.Where(p => !p.IsDeleted).ToListAsync();
        return entities.Select(ToDto);
    }

    public async Task<PoiResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.Pois.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        return entity is null ? null : ToDto(entity);
    }

    private static PoiResponseDto ToDto(PoiEntity p) =>
        new(p.Id, p.Name, p.WorkingHours, GeometryConverter.ToWkt(p.Geometry), p.CategoryId, p.UserId, p.CreatedDate);
}