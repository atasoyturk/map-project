namespace BackendApi.DTOs.Geo;

public sealed class SearchQueryDto
{
    public string?   Name       { get; init; }
    public string?   Type       { get; init; }
    public DateTime? StartDate  { get; init; }
    public DateTime? EndDate    { get; init; }
    public string    SortBy     { get; init; } = "creationDate";
    public string    SortOrder  { get; init; } = "asc";
}