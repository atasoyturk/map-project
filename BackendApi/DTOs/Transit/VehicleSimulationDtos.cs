namespace BackendApi.DTOs.Transit;

public sealed record VehiclePositionDto(
    int    RouteId,
    int    VehicleId,
    string PlateNumber,
    double Latitude,
    double Longitude,
    double ProgressPercentage,
    bool   Completed);

public sealed record VehicleActionResultDto(
    int     VehicleId,
    string  PlateNumber,
    bool    Success,
    string? Error);

public sealed record SimulationActionResponseDto(
    IList<VehicleActionResultDto> Results);
public sealed record SimulationVehicleIdsDto(
    [System.ComponentModel.DataAnnotations.Required] int[] VehicleIds);