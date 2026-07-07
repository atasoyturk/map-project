using BackendApi.DTOs;

namespace BackendApi.Services;

public interface ISearchService
{
    Task<SearchResponseDto> SearchAsync(SearchQueryDto query, int userId);
}