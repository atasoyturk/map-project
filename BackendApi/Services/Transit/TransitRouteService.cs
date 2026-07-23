using BackendApi.Data;
using BackendApi.DTOs.Transit;
using BackendApi.Entities.Transit;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Transit;

public sealed class TransitRouteService : ITransitRouteService
{
    private readonly AppDbContext _context;
    private readonly IOsrmService                 _osrmService;
    private readonly ILogger<TransitRouteService>  _logger;

    public TransitRouteService(AppDbContext context, IOsrmService osrmService, ILogger<TransitRouteService> logger)
    {
        _context     = context;
        _osrmService = osrmService;
        _logger      = logger;
    }
    
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

        return new TransitRouteDetailDto(
            entity.Id, entity.Name, entity.Color, entity.UserId, entity.CreatedDate,
            entity.RouteGeometry is null ? null : GeometryConverter.ToWkt(entity.RouteGeometry),
            stopDtos);

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
        
        await TryGenerateRouteAsync(routeId);

        return true;
    }

    public async Task<TransitRouteResponseDto?> GenerateRouteAsync(int routeId)
    {
        var entity = await _context.TransitRoutes
            .FirstOrDefaultAsync(r => r.Id == routeId && !r.IsDeleted);
        if (entity is null) return null;

        var stops = await _context.TransitStops
            .Where(s => s.TransitRouteId == routeId && !s.IsDeleted)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();

        if (stops.Count < 2)
            throw new ArgumentException("Rota oluşturmak için en az 2 durak gereklidir.");

        var coordinates = stops.Select(s => s.Geometry.Coordinate);

        var (success, routeGeometry, error) = await _osrmService.GetRouteAsync(coordinates);
        if (!success || routeGeometry is null)
            throw new InvalidOperationException(error ?? "Rota oluşturulamadı.");

        entity.RouteGeometry = routeGeometry;
        entity.ModifiedDate  = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> TryGenerateRouteAsync(int routeId)
    {
        try
        {
            var result = await GenerateRouteAsync(routeId);
            return result is not null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Route geometry could not be regenerated for TransitRoute {RouteId}", routeId);
            return false;
        }
    }

    private static TransitRouteResponseDto ToDto(TransitRoute r) =>
        new(r.Id, r.Name, r.Color, r.UserId, r.CreatedDate,
            r.RouteGeometry is null ? null : GeometryConverter.ToWkt(r.RouteGeometry));
}

    
