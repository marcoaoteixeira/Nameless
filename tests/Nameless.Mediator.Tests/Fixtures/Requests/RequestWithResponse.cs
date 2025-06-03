using Nameless.Mediator.Requests;

namespace Nameless.Mediator.Fixtures.Requests;

public record RequestWithResponse : IRequest<object> {
    public object ReturnValue { get; set; }
}

public class RequestWithResponseRequestHandler : IRequestHandler<RequestWithResponse, object> {
    public Task<object> HandleAsync(RequestWithResponse request, CancellationToken cancellationToken) {
        return Task.FromResult(request.ReturnValue);
    }
}