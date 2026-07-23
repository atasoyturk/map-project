// BackendApi/Services/Company/VehicleService.cs
using BackendApi.Data;
using BackendApi.DTOs.Company;
using BackendApi.Helpers;
using Microsoft.EntityFrameworkCore;
using VehicleEntity = BackendApi.Entities.Company.Vehicle;

namespace BackendApi.Services.Company;

public sealed class VehicleService : IVehicleService
{
    private readonly AppDbContext _context;

    public VehicleService(AppDbContext context) => _context = context;

    public async Task<VehicleResponseDto> CreateAsync(VehicleRequestDto request)
    {
        if (!await _context.Companies.AnyAsync(c => c.Id == request.CompanyId && !c.IsDeleted))
            throw new ArgumentException("Geçersiz şirket.");

        var plateExists = await _context.Vehicles
            .AnyAsync(v => v.PlateNumber == request.PlateNumber && !v.IsDeleted);
        if (plateExists)
            throw new InvalidOperationException("Bu plaka zaten kayıtlı.");

        var entity = new VehicleEntity
        {
            PlateNumber = request.PlateNumber,
            CompanyId   = request.CompanyId,
        };

        _context.Vehicles.Add(entity);
        await _context.SaveChangesAsync();

        return await ToDtoAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
        if (entity is null) return false;

        entity.SoftDelete(userId);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetAllAsync()
    {
        var vehicles = await (from v in _context.Vehicles
            join c in _context.Companies on v.CompanyId equals c.Id
            where !v.IsDeleted
            select new VehicleResponseDto(v.Id, v.PlateNumber, c.Id, c.Name))
            .ToListAsync();

        return vehicles;
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetByCompanyAsync(int companyId)
    {
        var vehicles = await (from v in _context.Vehicles
            join c in _context.Companies on v.CompanyId equals c.Id
            where !v.IsDeleted && v.CompanyId == companyId
            select new VehicleResponseDto(v.Id, v.PlateNumber, c.Id, c.Name))
            .ToListAsync();

        return vehicles;
    }

    private async Task<VehicleResponseDto> ToDtoAsync(VehicleEntity entity)
    {
        var companyName = await _context.Companies
            .Where(c => c.Id == entity.CompanyId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync() ?? string.Empty;

        return new VehicleResponseDto(entity.Id, entity.PlateNumber, entity.CompanyId, companyName);
    }
}