using BackendApi.DTOs;
using BackendApi.Services.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendApi.Controllers;

[Route("api/drawings")]
public sealed class SearchController : ApiControllerBase
{
    private readonly ISearchService            _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchService searchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger        = logger;
    }


    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchQueryDto query)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        try
        {
            var result = await _searchService.SearchAsync(query, userId.Value);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search failed");
            return StatusCode(500, "Sunucu hatası.");
        }
    }
}