using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nameless.Validation;

namespace Nameless.Microservices.App.Infrastructure;

/// <summary>
///     Represents a global exception handler for the application.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler {
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="GlobalExceptionHandler"/> class.
    /// </summary>
    /// <param name="problemDetailsService">
    ///     The service used to create problem details
    /// </param>
    /// <param name="logger">
    ///     The logger to use for logging exceptions.
    /// </param>
    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) {
        _problemDetailsService = Guard.Against.Null(problemDetailsService);
        _logger = Guard.Against.Null(logger);
    }

    /// <inheritdoc />
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
        _logger.GlobalExceptionHandlerExecution(exception);

        httpContext.Response.StatusCode = exception switch {
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return _problemDetailsService.TryWriteAsync(new ProblemDetailsContext {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails {
                Type = exception.GetType().Name,
                Title = "An error occurred while processing your request.",
                Detail = exception.Message,
                Status = httpContext.Response.StatusCode
            }
        });
    }
}
