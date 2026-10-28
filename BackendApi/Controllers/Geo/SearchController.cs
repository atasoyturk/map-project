using BackendApi.DTOs.Geo;
using BackendApi.Services.Geo;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Geo;

[Route("api/drawings")]
public sealed class SearchController : ApiControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
        => _searchService = searchService;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchQueryDto query)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _searchService.SearchAsync(query, userId.Value);
        return Ok(result);
    }
}