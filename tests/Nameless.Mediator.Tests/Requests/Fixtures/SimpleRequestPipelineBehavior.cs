using Microsoft.Extensions.Logging;

namespace Nameless.Mediator.Requests.Fixtures;

public class SimpleRequestPipelineBehavior : IRequestPipelineBehavior<SimpleRequest, Nothing> {
    private readonly ILogger _logger;

    public SimpleRequestPipelineBehavior(ILogger logger) {
        _logger = logger;
    }

    public Task<Nothing> HandleAsync(SimpleRequest request, RequestHandlerDelegate<Nothing> next, CancellationToken cancellationToken) {
        _logger.LogDebug("{RequestPipelineBehavior}: {Message}", GetType().Name, request.Message);

        return next(cancellationToken);
    }
}