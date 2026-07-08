using BackendApi.DTOs.Geo;
using NetTopologySuite.Geometries;

namespace BackendApi.Services.Geo;

public interface IGeoPermissionService
{
    // Boundary validation
    Task<bool> IsWithinBoundaryAsync(int userId, IEnumerable<string> roles, Geometry geometry);
    Task<bool> ValidateGeometryAsync(int userId, IEnumerable<string> roles, string wktGeometry);

    // GeoPermission CRUD
    Task<GeoPermissionResponseDto>              CreateAsync(GeoPermissionRequestDto request);
    Task<IEnumerable<GeoPermissionResponseDto>> GetAllAsync();
    Task<bool>                                  DeleteAsync(int id);
    Task<GeoPermissionResponseDto?>             UpdateAsync(int id, GeoPermissionRequestDto request);

    // User assignment
    Task<bool>                                  AssignToUserAsync(int userId, int geoPermissionId);
    Task<bool>                                  RemoveFromUserAsync(int userId, int geoPermissionId);
    Task<IEnumerable<GeoPermissionResponseDto>> GetByUserAsync(int userId);

    // Role assignment
    Task<bool>                                  AssignToRoleAsync(int roleId, int geoPermissionId);
    Task<bool>                                  RemoveFromRoleAsync(int roleId, int geoPermissionId);
    Task<IEnumerable<GeoPermissionResponseDto>> GetByRoleAsync(int roleId);
}