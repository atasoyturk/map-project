using BackendApi.DTOs.Analysis;

namespace BackendApi.Services.Analysis;

public interface IThematicMapService
{
    Task<IsochroneResultDto> GetIsochroneAsync(double centerLon, double centerLat, IReadOnlyList<int> minutesThresholds);
}