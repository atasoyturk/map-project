using BackendApi.Authorization;
using BackendApi.DTOs.Annotation;
using BackendApi.Services.Annotation;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers.Annotation;

[Route("api/annotation")]
public sealed class AnnotationController : ApiControllerBase
{
    private readonly IAnnotationService _annotationService;

    public AnnotationController(IAnnotationService annotationService)
        => _annotationService = annotationService;

    [HttpPost]
    [RequirePermission("annotation_create")]
    public async Task<IActionResult> Create([FromBody] AnnotationRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _annotationService.SaveAsync(request, userId.Value, GetTeamId(), GetUserRoles());
        return Ok(result);
    }

    [HttpGet]
    [RequirePermission("annotation_read")]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _annotationService.GetAllAsync(userId.Value, GetTeamId(), HasAdminAccess());
        return Ok(result);
    }
}