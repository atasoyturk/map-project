using BackendApi.DTOs.Geo;

namespace BackendApi.Services.Geo;

public interface ISearchService
{
    Task<SearchResponseDto> SearchAsync(SearchQueryDto query, int userId);
}