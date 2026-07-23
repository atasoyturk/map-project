using BackendApi.Data;
using BackendApi.DTOs.Company;
using BackendApi.DTOs.Transit;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;
using CompanyEntity = BackendApi.Entities.Company.Company;
using CompanyRouteEntity = BackendApi.Entities.Company.CompanyRoute;

namespace BackendApi.Services.Company;

public sealed class CompanyService : ICompanyService
{
    private readonly AppDbContext _context;

    public CompanyService(AppDbContext context) => _context = context;

    //  Company CRUD 

    public async Task<CompanyResponseDto> CreateAsync(CompanyRequestDto request)
    {
        if (!await _context.CompanyCategories.AnyAsync(c => c.Id == request.CompanyCategoryId && !c.IsDeleted))
            throw new ArgumentException("Geçersiz şirket kategorisi.");

        var entity = new CompanyEntity
        {
            Name              = request.Name,
            CompanyCategoryId = request.CompanyCategoryId,
        };

        _context.Companies.Add(entity);
        await _context.SaveChangesAsync();

        return await ToDtoAsync(entity);
    }

    public async Task<CompanyResponseDto?> UpdateAsync(int id, CompanyRequestDto request)
    {
        var entity = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity is null) return null;

        if (!await _context.CompanyCategories.AnyAsync(c => c.Id == request.CompanyCategoryId && !c.IsDeleted))
            throw new ArgumentException("Geçersiz şirket kategorisi.");

        entity.Name              = request.Name;
        entity.CompanyCategoryId = request.CompanyCategoryId;
        entity.ModifiedDate      = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await ToDtoAsync(entity);
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetAllAsync()
    {
        var companies = await (from c in _context.Companies
            join cat in _context.CompanyCategories on c.CompanyCategoryId equals cat.Id
            where !c.IsDeleted
            select new CompanyResponseDto(c.Id, c.Name, cat.Id, cat.Name))
            .ToListAsync();

        return companies;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity is null) return false;

        var hasActiveVehicles = await _context.Vehicles.AnyAsync(v => v.CompanyId == id && !v.IsDeleted);
        if (hasActiveVehicles)
            throw new InvalidOperationException("Bu şirkete bağlı araçlar var. Önce onları silin.");

        entity.SoftDelete(userId);
        await _context.SaveChangesAsync();
        return true;
    }

    //  Route assignment 

    public async Task<bool> AssignRouteAsync(int companyId, int transitRouteId)
    {
        var exists = await _context.CompanyRoutes
            .AnyAsync(cr => cr.CompanyId == companyId && cr.TransitRouteId == transitRouteId);

        if (exists) return true; // idempotent

        _context.CompanyRoutes.Add(new CompanyRouteEntity
        {
            CompanyId      = companyId,
            TransitRouteId = transitRouteId,
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveRouteAsync(int companyId, int transitRouteId)
    {
        var entity = await _context.CompanyRoutes
            .FirstOrDefaultAsync(cr => cr.CompanyId == companyId && cr.TransitRouteId == transitRouteId);

        if (entity is null) return false;

        _context.CompanyRoutes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TransitRouteResponseDto>> GetRoutesByCompanyAsync(int companyId)
    {
        var routes = await _context.CompanyRoutes
            .Where(cr => cr.CompanyId == companyId)
            .Include(cr => cr.TransitRoute)
            .Where(cr => !cr.TransitRoute.IsDeleted)
            .Select(cr => cr.TransitRoute)
            .ToListAsync();

        return routes.Select(r => new TransitRouteResponseDto(
            r.Id, r.Name, r.Color, r.UserId, r.CreatedDate,
            r.RouteGeometry is null ? null : GeometryConverter.ToWkt(r.RouteGeometry)));
    }

    public async Task<IEnumerable<TransitRouteResponseDto>> GetUnassignedRoutesAsync()
    {
        var assignedRouteIds = _context.CompanyRoutes.Select(cr => cr.TransitRouteId);

        var routes = await _context.TransitRoutes
            .Where(r => !r.IsDeleted && !assignedRouteIds.Contains(r.Id))
            .ToListAsync();

        return routes.Select(r => new TransitRouteResponseDto(
            r.Id, r.Name, r.Color, r.UserId, r.CreatedDate,
            r.RouteGeometry is null ? null : GeometryConverter.ToWkt(r.RouteGeometry)));
    }

    public async Task<IEnumerable<CompanyStatsDto>> GetStatsAsync()
    {
        var stats = await (from c in _context.Companies
            where !c.IsDeleted
            select new CompanyStatsDto(
                c.Id,
                c.Name,
                _context.Vehicles.Count(v => v.CompanyId == c.Id && !v.IsDeleted),
                _context.CompanyRoutes.Count(cr => cr.CompanyId == c.Id),
                _context.ShipmentRecords.Count(s =>
                    _context.Vehicles.Any(v => v.Id == s.VehicleId && v.CompanyId == c.Id))))
            .ToListAsync();

        return stats;
    }

    public async Task<IEnumerable<ShipmentRecordDto>> GetShipmentRecordsAsync(int? transitRouteId)
    {
        IQueryable<Entities.Transit.ShipmentRecord> query = _context.ShipmentRecords;

        if (transitRouteId.HasValue)
            query = query.Where(s => s.TransitRouteId == transitRouteId.Value);

        var records = await (from s in query
            join r in _context.TransitRoutes on s.TransitRouteId equals r.Id
            join v in _context.Vehicles on s.VehicleId equals v.Id
            join c in _context.Companies on v.CompanyId equals c.Id
            orderby s.CompletedAtUtc descending
            select new ShipmentRecordDto(
                s.Id, r.Id, r.Name, v.Id, v.PlateNumber, c.Name, s.StartedAtUtc, s.CompletedAtUtc))
            .ToListAsync();

        return records;
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetCompaniesByRouteAsync(int transitRouteId)
    {
        var companies = await _context.CompanyRoutes
            .Where(cr => cr.TransitRouteId == transitRouteId)
            .Include(cr => cr.Company)
            .Where(cr => !cr.Company.IsDeleted)
            .Select(cr => cr.Company)
            .ToListAsync();

        var results = new List<CompanyResponseDto>();
        foreach (var company in companies)
            results.Add(await ToDtoAsync(company));

        return results;
    }

    //  Helper 

    private async Task<CompanyResponseDto> ToDtoAsync(CompanyEntity entity)
    {
        var categoryName = await _context.CompanyCategories
            .Where(c => c.Id == entity.CompanyCategoryId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync() ?? string.Empty;

        return new CompanyResponseDto(entity.Id, entity.Name, entity.CompanyCategoryId, categoryName);
    }
}