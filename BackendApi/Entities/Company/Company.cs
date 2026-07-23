using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Company;

[Table("tbl_company")]
public sealed class Company : BaseEntity
{
    public string Name              { get; set; } = string.Empty;
    public int    CompanyCategoryId { get; set; }

    public ICollection<CompanyRoute> CompanyRoutes { get; init; } = [];
}