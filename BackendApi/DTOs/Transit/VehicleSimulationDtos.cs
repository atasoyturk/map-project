namespace BackendApi.DTOs.Transit;

public sealed record VehiclePositionDto(
    int    RouteId,
    double Latitude,
    double Longitude,
    double ProgressPercentage,
    bool   Completed);