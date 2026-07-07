using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Geo;

[Table("tbl_polygon")]
public sealed class PolygonEntity : BaseEntity
{
    public Geometry  Geometry       { get; set; } = null!;
    public string    Name           { get; set; } = string.Empty;
    public string    Color          { get; set; } = string.Empty;
    public int       UserId         { get; set; }
}