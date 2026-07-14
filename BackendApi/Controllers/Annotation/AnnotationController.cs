using BackendApi.Authorization;
using BackendApi.DTOs.Annotation;
using BackendApi.Services.Annotation;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Annotation;

[Route("api/annotation")]
public sealed class AnnotationController : ApiControllerBase
{
    private readonly IAnnotationService _annotationService;
    private readonly ILogger<AnnotationController> _logger;

    public AnnotationController(IAnnotationService annotationService, ILogger<AnnotationController> logger)
    {
        _annotationService = annotationService;
        _logger            = logger;
    }

    [HttpPost]
    [RequirePermission("annotation_create")]
    public async Task<IActionResult> Create([FromBody] AnnotationRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var result = await _annotationService.SaveAsync(request, userId.Value, GetTeamId(), GetUserRoles());
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Annotation create failed");
            return StatusCode(500, "Sunucu hatası.");
        }
    }

    [HttpGet]
    [RequirePermission("annotation_read")]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var isAdmin = GetUserRoles().Contains("Admin");
        var result  = await _annotationService.GetAllAsync(userId.Value, GetTeamId(), HasAdminAccess());
        return Ok(result);
    }
}