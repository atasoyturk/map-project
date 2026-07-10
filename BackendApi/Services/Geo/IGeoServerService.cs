namespace BackendApi.Services.Geo;

public interface IGeoServerService
{
    Task<(bool Success, string? Content, string? ContentType, string? Error)>
        GetFeaturesAsync(string typeName, int userId);
}