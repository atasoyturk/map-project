using BackendApi.DTOs;

namespace BackendApi.Services;

public interface IAnalysisService
{
    Task<int> TempInventoryAsync(GeoRequestDto request, int userId);
}