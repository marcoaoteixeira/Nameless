using Nameless.Mediator.Requests;

namespace Nameless.Mediator.Fixtures.Requests;

public record RequestWithoutResponse : IRequest;

public class RequestWithoutResponseRequestHandler : IRequestHandler<RequestWithoutResponse> {
    public Task HandleAsync(RequestWithoutResponse request, CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}