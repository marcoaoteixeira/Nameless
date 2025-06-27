using Microsoft.Extensions.Logging;

namespace Nameless.Mediator.Requests.Fixtures;

public class SimpleRequestHandler : IRequestHandler<SimpleRequest> {
    private readonly ILogger _logger;

    public SimpleRequestHandler(ILogger logger) {
        _logger = logger;
    }

    public Task HandleAsync(SimpleRequest request, CancellationToken cancellationToken) {
        _logger.LogDebug("{RequestHandler}: {Message}", GetType().Name, request.Message);

        return Task.CompletedTask;
    }
}