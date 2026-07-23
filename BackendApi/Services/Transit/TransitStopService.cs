using BackendApi.Data;
using BackendApi.DTOs.Transit;
using BackendApi.Entities.Transit;
using BackendApi.Helpers;
using BackendApi.Services.Geo;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace BackendApi.Services.Transit;

public sealed class TransitStopService : ITransitStopService
{
    private readonly AppDbContext          _context;
    private readonly IGeoPermissionService _geoPermissionService;
    private readonly ITransitRouteService  _transitRouteService;

    public TransitStopService(
        AppDbContext          context,
        IGeoPermissionService geoPermissionService,
        ITransitRouteService  transitRouteService)
    {
        _context              = context;
        _geoPermissionService = geoPermissionService;
        _transitRouteService  = transitRouteService;
    }

    public async Task<TransitStopResponseDto> CreateAsync(TransitStopRequestDto request, int userId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana durak ekleme yetkiniz bulunmamaktadır");

        if (!await _context.TransitRoutes.AnyAsync(r => r.Id == request.TransitRouteId && !r.IsDeleted))
            throw new ArgumentException("Geçersiz güzergah.");

        var nextOrder = await _context.TransitStops
            .Where(s => s.TransitRouteId == request.TransitRouteId && !s.IsDeleted)
            .CountAsync();

        var entity = new TransitStop
        {
            Name           = request.Name,
            Geometry       = (Point)geometry,
            TransitRouteId = request.TransitRouteId,
            UserId         = userId,
            SortOrder      = nextOrder,
        };

        _context.TransitStops.Add(entity);
        await _context.SaveChangesAsync();
        await _transitRouteService.TryGenerateRouteAsync(entity.TransitRouteId);

        return ToDto(entity);
    }

    public async Task<TransitStopResponseDto?> UpdateAsync(int id, TransitStopRequestDto request, int userId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        if (!await _geoPermissionService.IsWithinBoundaryAsync(userId, roles, geometry))
            throw new UnauthorizedAccessException("Bu alana durak ekleme yetkiniz bulunmamaktadır");

        if (!await _context.TransitRoutes.AnyAsync(r => r.Id == request.TransitRouteId && !r.IsDeleted))
            throw new ArgumentException("Geçersiz güzergah.");

        var entity = await _context.TransitStops
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId && !s.IsDeleted);
        if (entity is null) return null;

        var oldRouteId = entity.TransitRouteId;

        entity.Name           = request.Name;
        entity.Geometry       = (Point)geometry;
        entity.TransitRouteId = request.TransitRouteId;
        entity.ModifiedDate   = DateTime.UtcNow;
        entity.ModifiedUserId = userId;

        await _context.SaveChangesAsync();
        await _transitRouteService.TryGenerateRouteAsync(entity.TransitRouteId);
        
        if (oldRouteId != entity.TransitRouteId)
            await _transitRouteService.TryGenerateRouteAsync(oldRouteId);
        
        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.TransitStops
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId && !s.IsDeleted);
        if (entity is null) return false;

        entity.SoftDelete(userId);
        await _context.SaveChangesAsync();

        await _transitRouteService.TryGenerateRouteAsync(entity.TransitRouteId);

        return true;
    }

    public async Task<IEnumerable<TransitStopResponseDto>> GetAllAsync()
    {
        var entities = await _context.TransitStops.Where(s => !s.IsDeleted).ToListAsync();
        return entities.Select(ToDto);
    }

    public async Task<TransitStopResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _context.TransitStops.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        return entity is null ? null : ToDto(entity);
    }

    internal static TransitStopResponseDto ToDto(TransitStop s) =>
        new(s.Id, s.Name, GeometryConverter.ToWkt(s.Geometry), s.TransitRouteId, s.UserId, s.SortOrder, s.CreatedDate);
}