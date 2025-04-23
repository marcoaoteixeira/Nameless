namespace Nameless.Mediator.Streams;

/// <summary>
/// Represents a handler to a stream request.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public interface IStreamHandler<in TRequest, out TResponse>
    where TRequest : IStream<TResponse> {
    /// <summary>
    /// Handles the stream request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="IAsyncEnumerable{T}"/> representing the stream of data.</returns>
    IAsyncEnumerable<TResponse> HandleAsync(TRequest request,
                                            CancellationToken cancellationToken);
}