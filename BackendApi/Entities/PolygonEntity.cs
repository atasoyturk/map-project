using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities;

[Table("tbl_polygon")]
public sealed class PolygonEntity
{
    public int      Id           { get; init; }
    public Geometry Geometry     { get; set; } = null!;
    public string   Name         {get; set;} = string.Empty;
    public string   Color        {get; set;} = string.Empty;
    public int      UserId       { get; set; }  
    public bool     IsActive     { get; set; } = true;  
    public bool     IsDeleted    { get; set; } = false;  
    public DateTime CreatedDate  { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    public int?     ModifiedUserId { get; set; }
}