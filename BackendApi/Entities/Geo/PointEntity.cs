using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Geo;

[Table("tbl_point")]
public sealed class PointEntity : BaseEntity
{
    public Geometry  Geometry       { get; set; } = null!;
    public string    Name           { get; set; } = string.Empty;
    public string    Color          { get; set; } = string.Empty;
    public int       UserId         { get; set; }
    public int?      TeamId         { get; set; }
}