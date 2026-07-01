using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities;

[Table("tbl_line")]
public sealed class LineEntity
{
    public int      Id       { get; init; }
    public Geometry Geometry { get; set; } = null!;
}