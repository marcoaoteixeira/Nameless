namespace Nameless.Patterns.Mediator.Streams;

/// <summary>
/// An interface that represents a stream handler invoker.
/// </summary>
public interface IStreamHandlerInvoker {
    /// <summary>
    /// Creates a stream of data from a request.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="IAsyncEnumerable{T}"/> representing the stream of data.</returns>
    IAsyncEnumerable<TResponse> CreateAsync<TResponse>(IStream<TResponse> request,
                                                       CancellationToken cancellationToken);
}