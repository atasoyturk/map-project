using BackendApi.DTOs.Transit;

namespace BackendApi.Services.Transit;

public interface IThematicMapService
{
    Task<IsochroneResultDto> GetIsochroneAsync(double centerLon, double centerLat);
}