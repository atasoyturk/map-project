using Microsoft.AspNetCore.Diagnostics;

namespace BackendApi.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, exception.Message),
            ArgumentException           => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidOperationException   => (StatusCodes.Status409Conflict, exception.Message),
            KeyNotFoundException        => (StatusCodes.Status404NotFound, exception.Message),
            _                           => (StatusCodes.Status500InternalServerError, "Sunucu hatası.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception at {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        else
            _logger.LogWarning("Handled exception ({StatusCode}) at {Method} {Path}: {Message}",
                statusCode, httpContext.Request.Method, httpContext.Request.Path, exception.Message);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsync(message, cancellationToken);
        return true;
    }
}