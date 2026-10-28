using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Poi;

[Table("tbl_poi_category")]
public sealed class PoiCategory : BaseEntity
{
    public string  Name             { get; set; } = string.Empty;
    public int?    ParentCategoryId { get; set; }
    [MaxLength(16)]
    public string? Icon             { get; set; }
}