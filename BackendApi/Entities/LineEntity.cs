using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities;

[Table("tbl_line")]
public sealed class LineEntity : BaseEntity
{
    public Geometry  Geometry       { get; set; } = null!;
    public string    Name           { get; set; } = string.Empty;
    public string    Color          { get; set; } = string.Empty;
    public int       UserId         { get; set; }
}