using BackendApi.DTOs.Company;

namespace BackendApi.Services.Company;

public interface IVehicleService
{
    Task<VehicleResponseDto>              CreateAsync(VehicleRequestDto request);
    Task<bool>                            DeleteAsync(int id, int userId);
    Task<IEnumerable<VehicleResponseDto>> GetAllAsync();
    Task<IEnumerable<VehicleResponseDto>> GetByCompanyAsync(int companyId);
}