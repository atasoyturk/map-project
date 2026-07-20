using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Transit;

[Table("tbl_transit_stop")]
public sealed class TransitStop : BaseEntity
{
    public string Name           { get; set; } = string.Empty;
    public Point  Geometry       { get; set; } = null!;
    public int    TransitRouteId { get; set; }
    public int    UserId         { get; set; }
    public int    SortOrder      { get; set; }
}