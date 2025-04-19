namespace Nameless.CQRS.Streams;

public interface IStreamer {
    IAsyncEnumerable<TResponse> StreamAsync<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken);
}