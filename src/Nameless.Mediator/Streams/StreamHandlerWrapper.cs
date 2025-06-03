namespace Nameless.Mediator.Streams;

/// <summary>
/// Base class for a stream handler wrapper.
/// </summary>
public abstract class StreamHandlerWrapperBase {
    /// <summary>
    /// Handles the stream request asynchronously.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A stream of responses as an <see cref="IAsyncEnumerable{T}"/>.
    /// </returns>
    public abstract IAsyncEnumerable<object?> HandleAsync(object request,
                                                          IServiceProvider serviceProvider,
                                                          CancellationToken cancellationToken);
}

/// <summary>
/// Base class for a stream handler wrapper.
/// </summary>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public abstract class StreamHandlerWrapper<TResponse> : StreamHandlerWrapperBase {
    /// <summary>
    /// Handles the stream request asynchronously.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A stream of responses as an <see cref="IAsyncEnumerable{T}"/>.
    /// </returns>
    public abstract IAsyncEnumerable<TResponse> HandleAsync(IStream<TResponse> request,
                                                            IServiceProvider serviceProvider,
                                                            CancellationToken cancellationToken);
}