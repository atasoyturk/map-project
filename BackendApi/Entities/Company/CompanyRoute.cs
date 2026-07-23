using BackendApi.Entities.Transit;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Company;

[Table("tbl_company_route")]
public sealed class CompanyRoute
{
    public int CompanyId      { get; init; }
    public int TransitRouteId { get; init; }

    public Company      Company      { get; init; } = null!;
    public TransitRoute TransitRoute { get; init; } = null!;
}