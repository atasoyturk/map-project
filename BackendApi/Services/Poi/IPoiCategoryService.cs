using BackendApi.DTOs.Poi;

namespace BackendApi.Services.Poi;

public interface IPoiCategoryService
{
    Task<PoiCategoryResponseDto>  CreateAsync(PoiCategoryRequestDto request);
    Task<PoiCategoryResponseDto?> UpdateAsync(int id, PoiCategoryRequestDto request);
    Task<bool>                    DeleteAsync(int id);
    Task<IList<PoiCategoryTreeDto>> GetTreeAsync();
}