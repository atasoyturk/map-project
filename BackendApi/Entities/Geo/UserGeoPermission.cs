using BackendApi.Entities.Auth;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Geo;

[Table("tbl_user_geo_permission")]
public sealed class UserGeoPermission
{
    public int  UserId            { get; init; }
    public int  GeoPermissionId   { get; init; }

    public User              User          { get; init; } = null!;
    public GeoPermissionEntity GeoPermission { get; init; } = null!;
}