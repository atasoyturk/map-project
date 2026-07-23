using BackendApi.DTOs.Company;

namespace BackendApi.Services.Company;

public interface ICompanyCategoryService
{
    Task<CompanyCategoryResponseDto>              CreateAsync(CompanyCategoryRequestDto request);
    Task<IEnumerable<CompanyCategoryResponseDto>> GetAllAsync();
    Task<bool>                                    DeleteAsync(int id, int userId);
}