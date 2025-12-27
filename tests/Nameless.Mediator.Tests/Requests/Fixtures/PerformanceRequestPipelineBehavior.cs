using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Nameless.Mediator.Requests.Fixtures;

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
    where TRequest : class
    where TResponse : class {
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="PerformanceRequestPipelineBehavior{TRequest,TResponse}"/> class.
    /// </summary>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public PerformanceRequestPipelineBehavior(ILogger logger) {
        _logger = Guard.Against.Null(logger);
    }

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation("Starting...");

        try { return await next(cancellationToken).SkipContextSync(); }
        finally { _logger.LogInformation("Finished! Time {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds); }
    }
}
