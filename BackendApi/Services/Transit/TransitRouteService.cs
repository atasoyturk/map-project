using BackendApi.Data;
using BackendApi.DTOs.Transit;
using BackendApi.Entities.Transit;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Transit;

public sealed class TransitRouteService : ITransitRouteService
{
    private readonly AppDbContext _context;

    public TransitRouteService(AppDbContext context) => _context = context;

    public async Task<TransitRouteResponseDto> CreateAsync(TransitRouteRequestDto request, int userId)
    {
        var entity = new TransitRoute
        {
            Name  = request.Name,
            Color = request.Color,
            UserId = userId,

        };

        _context.TransitRoutes.Add(entity);
        await _context.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<TransitRouteResponseDto?> UpdateAsync(int id, TransitRouteRequestDto request)
    {
        var entity = await _context.TransitRoutes
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (entity is null) return null;

        entity.Name         = request.Name;
        entity.Color        = request.Color;
        entity.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.TransitRoutes
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (entity is null) return false;

        var hasActiveStops = await _context.TransitStops
            .AnyAsync(s => s.TransitRouteId == id && !s.IsDeleted);
        if (hasActiveStops)
            throw new InvalidOperationException("Bu güzergaha bağlı duraklar var. Önce onları silin veya başka güzergaha taşıyın.");

        entity.SoftDelete(userId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TransitRouteResponseDto>> GetAllAsync()
    {
        var entities = await _context.TransitRoutes.Where(r => !r.IsDeleted).ToListAsync();
        return entities.Select(ToDto);
    }

    public async Task<TransitRouteDetailDto?> GetDetailAsync(int id)
    {
        var entity = await _context.TransitRoutes
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (entity is null) return null;

        var stops = await _context.TransitStops
            .Where(s => s.TransitRouteId == id && !s.IsDeleted)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();

        var stopDtos = stops.Select(TransitStopService.ToDto).ToList();

        return new TransitRouteDetailDto(entity.Id, entity.Name, entity.Color, entity.UserId, entity.CreatedDate, stopDtos);
    }

    public async Task<bool> ReorderStopsAsync(int routeId, int[] stopIdsInOrder)
    {
        var stops = await _context.TransitStops
            .Where(s => s.TransitRouteId == routeId && !s.IsDeleted && stopIdsInOrder.Contains(s.Id))
            .ToListAsync();

        if (stops.Count != stopIdsInOrder.Length)
            throw new ArgumentException("Durak listesi güzergahla eşleşmiyor.");

        var now = DateTime.UtcNow;
        for (var i = 0; i < stopIdsInOrder.Length; i++)
        {
            var stop = stops.First(s => s.Id == stopIdsInOrder[i]);
            stop.SortOrder    = i;
            stop.ModifiedDate = now;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private static TransitRouteResponseDto ToDto(TransitRoute r) =>
        new(r.Id, r.Name, r.Color, r.UserId, r.CreatedDate);
}