using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SeatLock.Application.Common.Exceptions;

namespace SeatLock.WebApi.Middleware;

public sealed partial class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problem;
        switch (exception)
        {
            case RequestValidationException validationException:
                problem = new ValidationProblemDetails(validationException.Errors.ToDictionary(pair => pair.Key, pair => pair.Value))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed."
                };
                break;
            case UnauthorizedException unauthorizedException:
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = unauthorizedException.Message
                };
                break;
            default:
                LogUnhandledException(logger, exception, httpContext.Request.Path);
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred."
                };
                break;
        }

        httpContext.Response.StatusCode = problem.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    [LoggerMessage(LogLevel.Error, "Unhandled exception for {Path}")]
    private static partial void LogUnhandledException(ILogger logger, Exception exception, PathString path);
}
