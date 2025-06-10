using Microsoft.Extensions.Logging;
using Nameless.Mediator.Requests;

namespace Nameless.Mediator.Fixtures.Requests;

public record RequestWithoutResponse : IRequest {
    public int Value { get; set; }
}

public class RequestWithoutResponseRequestHandler : IRequestHandler<RequestWithoutResponse> {
    private readonly ILogger<RequestWithoutResponseRequestHandler> _logger;

    public RequestWithoutResponseRequestHandler(ILogger<RequestWithoutResponseRequestHandler> logger) {
        _logger = logger;
    }

    public Task HandleAsync(RequestWithoutResponse request, CancellationToken cancellationToken) {
        _logger.LogInformation("Request value: {RequestValue}", request.Value);

        return Task.CompletedTask;
    }
}