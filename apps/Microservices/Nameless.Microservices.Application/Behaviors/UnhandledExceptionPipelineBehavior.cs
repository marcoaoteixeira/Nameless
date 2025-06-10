using Microsoft.Extensions.Logging;
using Nameless.Mediator.Requests;

namespace Nameless.Microservices.Application.Behaviors;

/// <summary>
/// Unhandled exception behavior. 
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class UnhandledExceptionPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequestBase {

    private readonly ILogger<UnhandledExceptionPipelineBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="UnhandledExceptionPipelineBehavior{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public UnhandledExceptionPipelineBehavior(ILogger<UnhandledExceptionPipelineBehavior<TRequest, TResponse>> logger) {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        try { return await next(cancellationToken); }
        catch (Exception ex) {
            _logger.RequestHandlerUnhandledException(request, ex);

            throw;
        }
    }
}
