using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Entities.Team;

[Table("tbl_team")]
public sealed class Team : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}