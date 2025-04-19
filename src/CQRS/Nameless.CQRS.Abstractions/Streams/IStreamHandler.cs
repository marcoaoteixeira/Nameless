namespace Nameless.CQRS.Streams;

public interface IStreamHandler<in TRequest, out TResponse>
    where TRequest : IStreamRequest<TResponse> {
    IAsyncEnumerable<TResponse> HandleAsync(TRequest request,
                                            CancellationToken cancellationToken);
}