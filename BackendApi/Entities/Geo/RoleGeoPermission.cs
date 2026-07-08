using BackendApi.Entities.Auth;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Geo;

[Table("tbl_role_geo_permission")]
public sealed class RoleGeoPermission
{
    public int  RoleId            { get; init; }
    public int  GeoPermissionId   { get; init; }

    public Role              Role          { get; init; } = null!;
    public GeoPermissionEntity GeoPermission { get; init; } = null!;
}