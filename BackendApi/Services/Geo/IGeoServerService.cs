namespace BackendApi.Services.Geo;

public interface IGeoServerService
{
    Task<(bool Success, string? Content, string? ContentType, string? Error)>
        GetFeaturesAsync(string typeName, int userId);

    Task<(bool Success, byte[]? Content, string? ContentType, string? Error)>
        GetWmsMapAsync(string typeName, int userId, string bbox, int width, int height, string? styles);
}