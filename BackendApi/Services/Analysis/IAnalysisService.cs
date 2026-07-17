using BackendApi.DTOs.Geo;

namespace BackendApi.Services.Analysis;
using BackendApi.DTOs.Analysis;


public interface IAnalysisService
{
    Task<int> TempInventoryAsync(GeoRequestDto request, int userId);
    Task<LocationAnalysisResponseDto> LocationAnalysisAsync(LocationAnalysisRequestDto request, int userId);

}