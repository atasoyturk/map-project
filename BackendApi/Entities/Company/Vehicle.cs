using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Company;

[Table("tbl_vehicle")]
public sealed class Vehicle : BaseEntity
{
    public string PlateNumber { get; set; } = string.Empty;
    public int    CompanyId   { get; set; }
}