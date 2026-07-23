// BackendApi/Services/Company/ICompanyService.cs
using BackendApi.DTOs.Company;
using BackendApi.DTOs.Transit;

namespace BackendApi.Services.Company;

public interface ICompanyService
{
    Task<CompanyResponseDto>              CreateAsync(CompanyRequestDto request);
    Task<CompanyResponseDto?>             UpdateAsync(int id, CompanyRequestDto request);
    Task<IEnumerable<CompanyResponseDto>> GetAllAsync();
    Task<bool>                            DeleteAsync(int id, int userId);

    Task<bool>                                  AssignRouteAsync(int companyId, int transitRouteId);
    Task<bool>                                  RemoveRouteAsync(int companyId, int transitRouteId);
    Task<IEnumerable<TransitRouteResponseDto>>  GetRoutesByCompanyAsync(int companyId);
    Task<IEnumerable<CompanyResponseDto>>       GetCompaniesByRouteAsync(int transitRouteId);
    Task<IEnumerable<CompanyStatsDto>>    GetStatsAsync();
    Task<IEnumerable<ShipmentRecordDto>>  GetShipmentRecordsAsync(int? transitRouteId);
    Task<IEnumerable<TransitRouteResponseDto>> GetUnassignedRoutesAsync();
}