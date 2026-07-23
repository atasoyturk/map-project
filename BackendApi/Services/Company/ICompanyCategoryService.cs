using BackendApi.DTOs.Company;
using BackendApi.DTOs.Transit;

namespace BackendApi.Services.Company;

public interface ICompanyCategoryService
{
    Task<CompanyCategoryResponseDto>              CreateAsync(CompanyCategoryRequestDto request);
    Task<IEnumerable<CompanyCategoryResponseDto>> GetAllAsync();
    Task<bool>                                    DeleteAsync(int id, int userId);
    Task<IEnumerable<TransitRouteResponseDto>>    GetRoutesByCategoryAsync(int categoryId);
}