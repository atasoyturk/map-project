using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace BackendApi.Entities.Transit;

[Table("tbl_transit_route")]
public sealed class TransitRoute : BaseEntity
{
    public string    Name           { get; set; } = string.Empty;
    public string    Color          { get; set; } = string.Empty;
    public int?      UserId         { get; set; }
    public Geometry? RouteGeometry  { get; set; }
}