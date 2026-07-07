using NetTopologySuite.Geometries;

namespace BackendApi.Services.Geo;

public interface IGeoPermissionService
{
    Task<bool> IsWithinBoundaryAsync(int userId, IEnumerable<string> roles, Geometry geometry);
}