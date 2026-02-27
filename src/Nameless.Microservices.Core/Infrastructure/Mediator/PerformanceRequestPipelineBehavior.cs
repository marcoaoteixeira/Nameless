using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nameless.Mediator.Requests;

namespace Nameless.Microservices.Infrastructure.Mediator;

/// <summary>
///     A performance request pipeline behavior that logs the amount
///     of time that a request handler takes to finish.
/// </summary>
/// <typeparam name="TRequest">
///     Type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public class PerformanceRequestPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull {
    private readonly ILogger<PerformanceRequestPipelineBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="PerformanceRequestPipelineBehavior{TRequest,TResponse}"/> class.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public PerformanceRequestPipelineBehavior(ILogger<PerformanceRequestPipelineBehavior<TRequest, TResponse>> logger) {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        var sw = Stopwatch.StartNew();

        _logger.PerformanceRequestPipelineBehaviorStarting();

        try { return await next(cancellationToken).SkipContextSync(); }
        finally { _logger.PerformanceRequestPipelineBehaviorFinished(sw.ElapsedMilliseconds); }
    }
}
