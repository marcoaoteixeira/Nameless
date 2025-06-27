using Microsoft.Extensions.Logging;

namespace Nameless.Mediator.Requests.Fixtures;

public class SimpleRequestResponseHandler : IRequestHandler<SimpleRequestResponse, string> {
    private readonly ILogger _logger;

    public SimpleRequestResponseHandler(ILogger logger) {
        _logger = logger;
    }

    public Task<string> HandleAsync(SimpleRequestResponse request, CancellationToken cancellationToken) {
        _logger.LogDebug("{RequestHandler}: {Message}", GetType().Name, request.Message);

        return Task.FromResult(request.Message);
    }
}