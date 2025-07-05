using Microsoft.Extensions.Logging;

namespace Nameless.Mediator.Streams.Fixtures;

public class YetAnotherSimpleStreamPipelineBehavior : IStreamPipelineBehavior<SimpleStream, string> {
    private readonly ILogger _logger;

    public YetAnotherSimpleStreamPipelineBehavior(ILogger logger) {
        _logger = logger;
    }

    public IAsyncEnumerable<string> HandleAsync(SimpleStream request, StreamHandlerDelegate<string> next, CancellationToken cancellationToken) {
        foreach (var message in request.Messages) {
            _logger.LogDebug("{StreamPipelineBehavior}: {Message}", GetType().Name, message);
        }

        return next();
    }
}