using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Entities;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services;

public sealed class SearchService : ISearchService
{
    private readonly AppDbContext _context;

    public SearchService(AppDbContext context) => _context = context;

    public async Task<SearchResponseDto> SearchAsync(SearchQueryDto q, int userId)
    {
        return new SearchResponseDto(
            await SearchPoints(q, userId),
            await SearchLines(q, userId),
            await SearchPolygons(q, userId)
        );
    }

    private async Task<IEnumerable<PointResponseDto>> SearchPoints(SearchQueryDto q, int userId)
    {
        IQueryable<PointEntity> query = _context.Points
            .Where(p => p.UserId == userId && !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(q.Name))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{q.Name}%"));
        if (q.StartDate.HasValue)
            query = query.Where(p => p.CreatedDate >= q.StartDate.Value);
        if (q.EndDate.HasValue)
            query = query.Where(p => p.CreatedDate <= q.EndDate.Value);

        query = q.SortBy == "name"
            ? (q.SortOrder == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name))
            : (q.SortOrder == "asc" ? query.OrderBy(p => p.CreatedDate) : query.OrderByDescending(p => p.CreatedDate));

        return await query
            .Select(p => new PointResponseDto(p.Id, p.Name, p.Color, GeometryConverter.ToWkt(p.Geometry)))
            .ToListAsync();
    }

    private async Task<IEnumerable<LineResponseDto>> SearchLines(SearchQueryDto q, int userId)
    {
        IQueryable<LineEntity> query = _context.Lines
            .Where(l => l.UserId == userId && !l.IsDeleted);

        if (!string.IsNullOrWhiteSpace(q.Name))
            query = query.Where(l => EF.Functions.ILike(l.Name, $"%{q.Name}%"));
        if (q.StartDate.HasValue)
            query = query.Where(l => l.CreatedDate >= q.StartDate.Value);
        if (q.EndDate.HasValue)
            query = query.Where(l => l.CreatedDate <= q.EndDate.Value);

        query = q.SortBy == "name"
            ? (q.SortOrder == "asc" ? query.OrderBy(l => l.Name) : query.OrderByDescending(l => l.Name))
            : (q.SortOrder == "asc" ? query.OrderBy(l => l.CreatedDate) : query.OrderByDescending(l => l.CreatedDate));

        return await query
            .Select(l => new LineResponseDto(l.Id, l.Name, l.Color, GeometryConverter.ToWkt(l.Geometry)))
            .ToListAsync();
    }

    private async Task<IEnumerable<PolygonResponseDto>> SearchPolygons(SearchQueryDto q, int userId)
    {
        IQueryable<PolygonEntity> query = _context.Polygons
            .Where(p => p.UserId == userId && !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(q.Name))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{q.Name}%"));
        if (q.StartDate.HasValue)
            query = query.Where(p => p.CreatedDate >= q.StartDate.Value);
        if (q.EndDate.HasValue)
            query = query.Where(p => p.CreatedDate <= q.EndDate.Value);

        query = q.SortBy == "name"
            ? (q.SortOrder == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name))
            : (q.SortOrder == "asc" ? query.OrderBy(p => p.CreatedDate) : query.OrderByDescending(p => p.CreatedDate));

        return await query
            .Select(p => new PolygonResponseDto(p.Id, p.Name, p.Color, GeometryConverter.ToWkt(p.Geometry), 0))
            .ToListAsync();
    }
}