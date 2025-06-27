using Microsoft.Extensions.Logging;

namespace Nameless.Mediator.Requests.Fixtures;

public class SimpleRequestResponsePipelineBehavior : IRequestPipelineBehavior<SimpleRequestResponse, string> {
    private readonly ILogger _logger;

    public SimpleRequestResponsePipelineBehavior(ILogger logger) {
        _logger = logger;
    }

    public Task<string> HandleAsync(SimpleRequestResponse request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) {
        _logger.LogDebug("{RequestPipelineBehavior}: {Message}", GetType().Name, request.Message);

        return next(cancellationToken);
    }
}