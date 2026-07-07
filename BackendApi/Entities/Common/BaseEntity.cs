namespace BackendApi.Entities;

public abstract class BaseEntity
{
    public int      Id             { get; init; }
    public bool     IsActive       { get; set; } = true;
    public bool     IsDeleted      { get; set; } = false;
    public DateTime CreatedDate    { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedDate   { get; set; } = DateTime.UtcNow;
    public int?     ModifiedUserId { get; set; }
}