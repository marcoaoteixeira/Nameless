using Nameless.Mediator.Fixtures;

namespace Nameless.Mediator.Requests.Fixtures;

public class OpenGenericRequestPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : class {
    private readonly IPrintService _printService;

    public OpenGenericRequestPipelineBehavior(IPrintService printService) {
        _printService = printService;
    }

    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        _printService.Print($"OpenGenericRequestPipelineBehavior: {GetType().Name} | Request: {typeof(TRequest).Name} | Response: {typeof(TResponse).Name}");

        return next(cancellationToken);
    }
}