using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace BackendApi.Entities.Geo;

[Table("tbl_geo_permission")]
public sealed class GeoPermissionEntity : BaseEntity
{
    public string   Name             { get; set; } = string.Empty;
    public Geometry BoundaryGeometry { get; set; } = null!;

    // Navigation properties
    public ICollection<UserGeoPermission>  UserGeoPermissions { get; init; } = [];
    public ICollection<RoleGeoPermission>  RoleGeoPermissions { get; init; } = [];
}