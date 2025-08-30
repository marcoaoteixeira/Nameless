namespace Nameless.Mediator.Streams;

/// <summary>
///     Defines a stream handler wrapper.
/// </summary>
public abstract class StreamHandlerWrapper {
    /// <summary>
    ///     Handles the stream request asynchronously.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    ///     </param>
    /// <returns>
    ///     An <see cref="IAsyncEnumerable{T}" /> representing the stream of data.
    /// </returns>
    public abstract IAsyncEnumerable<object?> HandleAsync(object request, IServiceProvider provider, CancellationToken cancellationToken);
}

/// <summary>
///     Defines a generic stream handler wrapper.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public abstract class StreamHandlerWrapper<TResponse> : StreamHandlerWrapper {
    /// <summary>
    ///     Handles the stream request asynchronously.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    ///     </param>
    /// <returns>
    ///     An <see cref="IAsyncEnumerable{T}" /> representing the stream of data.
    /// </returns>
    public abstract IAsyncEnumerable<TResponse> HandleAsync(IStream<TResponse> request, IServiceProvider provider, CancellationToken cancellationToken);
}