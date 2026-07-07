using BackendApi.Entities.Auth;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Geo;

[Table("tbl_geo_permission")]
public sealed class GeoPermissionEntity : BaseEntity
{
    public int?     UserId           { get; set; }  
    public int?     RoleId           { get; set; }  
    public Geometry BoundaryGeometry { get; set; } = null!;

    // Navigation properties
    public User? User { get; init; }
    public Role? Role { get; init; }
}