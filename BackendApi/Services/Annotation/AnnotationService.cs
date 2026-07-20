using BackendApi.Data;
using BackendApi.DTOs.Annotation;
using BackendApi.Helpers;
using BackendApi.Services.Geo;
using Microsoft.EntityFrameworkCore;
using AnnotationEntity = BackendApi.Entities.Annotation.Annotation;

namespace BackendApi.Services.Annotation;

public sealed class AnnotationService : IAnnotationService
{
    private readonly AppDbContext _context;
    private readonly IGeoPermissionService _geoPermissionService;

    public AnnotationService(AppDbContext context, IGeoPermissionService geoPermissionService)
    {
        _context              = context;
        _geoPermissionService = geoPermissionService;
    }

    public async Task<AnnotationResponseDto> SaveAsync(AnnotationRequestDto request, int userId, int? teamId, IEnumerable<string> roles)
    {
        var geometry = GeometryConverter.FromWkt(request.WktGeometry);

        await _geoPermissionService.EnsureWithinBoundaryAsync(
            userId, roles, geometry, "Bu alana not ekleme yetkiniz bulunmamaktadır");

        var entity = new AnnotationEntity
        {
            NoteText = request.NoteText,
            Geometry = geometry,
            UserId   = userId,
            TeamId   = teamId
        };

        _context.Annotations.Add(entity);
        await _context.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<IEnumerable<AnnotationResponseDto>> GetAllAsync(int userId, int? teamId, bool isAdmin)
    {
        IQueryable<AnnotationEntity> query = _context.Annotations.Where(a => !a.IsDeleted);

        if (!isAdmin)
            query = query.Where(a => a.TeamId == null || a.TeamId == teamId);

        var entities = await query.ToListAsync();
        return entities.Select(ToDto);
    }

    public async Task<bool> DeleteAsync(int id, int userId, IEnumerable<string> roles)
    {
        var isAdmin = roles.Contains("Admin");
        Console.WriteLine($"Silinecek ID: {id}");
        Console.WriteLine($"Giriş yapan UserId: {userId}");
        Console.WriteLine($"Roller: {string.Join(",", roles)}");
        Console.WriteLine($"Admin mi: {isAdmin}");
        
        var entity = await _context.Annotations
            .FirstOrDefaultAsync(a => (isAdmin || a.UserId == userId) && a.Id == id && !a.IsDeleted);

        if (entity is null) return false;

        entity.IsDeleted = true;
        entity.ModifiedDate = DateTime.UtcNow;
        entity.ModifiedUserId = userId;
        await _context.SaveChangesAsync();

        return true;
    }


    private static AnnotationResponseDto ToDto(AnnotationEntity entity) =>
        new(entity.Id, entity.NoteText, GeometryConverter.ToWkt(entity.Geometry),
            entity.UserId, entity.TeamId, entity.CreatedDate);
}