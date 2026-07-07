using BackendApi.DTOs.Geo;

namespace BackendApi.Services.Search;

public interface ISearchService
{
    Task<SearchResponseDto> SearchAsync(SearchQueryDto query, int userId);
}