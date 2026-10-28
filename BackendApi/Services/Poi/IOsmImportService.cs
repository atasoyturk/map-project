using BackendApi.DTOs.Poi;

namespace BackendApi.Services.Poi;

public interface IOsmImportService
{
    // Error is set (Result null) when Overpass fails; the controller maps it to 502.
    Task<(OsmImportResultDto? Result, string? Error)> ImportAsync(
        OsmImportRequestDto request, int userId, CancellationToken cancellationToken);
}
