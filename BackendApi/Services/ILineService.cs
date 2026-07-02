using BackendApi.DTOs;

namespace BackendApi.Services;

public interface ILineService
{
    Task<LineResponseDto> SaveAsync(GeoRequestDto request);
}