namespace Nameless.CQRS.Requests;

public interface IRequester {
    Task ExecuteAsync<TRequest>(TRequest request,
                                CancellationToken cancellationToken)
        where TRequest : IRequest;

    Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request,
                                            CancellationToken cancellationToken);
}