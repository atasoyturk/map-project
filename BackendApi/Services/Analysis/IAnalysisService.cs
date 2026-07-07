using BackendApi.DTOs;

namespace BackendApi.Services.Analysis;

public interface IAnalysisService
{
    Task<int> TempInventoryAsync(GeoRequestDto request, int userId);
}