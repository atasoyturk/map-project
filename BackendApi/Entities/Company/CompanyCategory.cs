using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Company;

[Table("tbl_company_category")]
public sealed class CompanyCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}