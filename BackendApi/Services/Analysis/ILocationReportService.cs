using BackendApi.DTOs.Analysis;

namespace BackendApi.Services.Analysis;

public interface ILocationReportService
{
    Task<LocationReportDto> GetReportAsync(LocationReportRequestDto request);
}
