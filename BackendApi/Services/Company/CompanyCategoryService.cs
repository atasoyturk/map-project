using BackendApi.Data;
using BackendApi.DTOs.Company;
using BackendApi.DTOs.Transit;
using BackendApi.Helpers;
using CompanyCategoryEntity = BackendApi.Entities.Company.CompanyCategory;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Services.Company;

public sealed class CompanyCategoryService : ICompanyCategoryService
{
    private readonly AppDbContext _context;

    public CompanyCategoryService(AppDbContext context) => _context = context;

    public async Task<CompanyCategoryResponseDto> CreateAsync(CompanyCategoryRequestDto request)
    {
        var entity = new CompanyCategoryEntity { Name = request.Name };

        _context.CompanyCategories.Add(entity);
        await _context.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<IEnumerable<CompanyCategoryResponseDto>> GetAllAsync()
    {
        var entities = await _context.CompanyCategories.Where(c => !c.IsDeleted).ToListAsync();
        return entities.Select(ToDto);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.CompanyCategories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity is null) return false;

        var hasActiveCompanies = await _context.Companies
            .AnyAsync(c => c.CompanyCategoryId == id && !c.IsDeleted);
        if (hasActiveCompanies)
            throw new InvalidOperationException("Bu kategoriye bağlı şirketler var. Önce onları başka bir kategoriye taşıyın veya silin.");

        entity.SoftDelete(userId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TransitRouteResponseDto>> GetRoutesByCategoryAsync(int categoryId)
    {
        var companyIds = _context.Companies
            .Where(c => c.CompanyCategoryId == categoryId && !c.IsDeleted)
            .Select(c => c.Id);

        var routes = await _context.CompanyRoutes
            .Where(cr => companyIds.Contains(cr.CompanyId))
            .Include(cr => cr.TransitRoute)
            .Where(cr => !cr.TransitRoute.IsDeleted)
            .Select(cr => cr.TransitRoute)
            .Distinct()
            .ToListAsync();

        return routes.Select(r => new TransitRouteResponseDto(
            r.Id, r.Name, r.Color, r.UserId, r.CreatedDate,
            r.RouteGeometry is null ? null : GeometryConverter.ToWkt(r.RouteGeometry)));
    }

    private static CompanyCategoryResponseDto ToDto(CompanyCategoryEntity entity) =>
        new(entity.Id, entity.Name);
}