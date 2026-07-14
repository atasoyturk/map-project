using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Poi;

[Table("tbl_poi")]
public sealed class Poi : BaseEntity
{
    public string   Name         { get; set; } = string.Empty;
    public string   WorkingHours { get; set; } = string.Empty;
    public Point    Geometry     { get; set; } = null!;
    public int      CategoryId   { get; set; }
    public int      UserId       { get; set; }
}