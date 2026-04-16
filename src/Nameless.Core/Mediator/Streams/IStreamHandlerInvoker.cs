namespace Nameless.Mediator.Streams;

/// <summary>
///     Defines a stream handler invoker.
/// </summary>
public interface IStreamHandlerInvoker {
    /// <summary>
    ///     Creates a stream of data for the request object.
    /// </summary>
    /// <typeparam name="TResponse">
    ///     Type of the response.
    /// </typeparam>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncEnumerable{T}" /> representing the stream of data.
    /// </returns>
    IAsyncEnumerable<TResponse> CreateAsync<TResponse>(IStream<TResponse> request, CancellationToken cancellationToken);
}