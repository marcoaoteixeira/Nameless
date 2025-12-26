namespace Nameless.Mediator.Streams;

/// <summary>
///     Defines a pipeline behavior to surround the stream handler and
///     adds additional behavior.
/// </summary>
/// <typeparam name="TRequest">
///     Type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public interface IStreamPipelineBehavior<in TRequest, TResponse> where TRequest : notnull {
    /// <summary>
    ///     Executes the pipeline handler performing any additional
    ///     behavior, then executes the <paramref name="next" /> delegate,
    ///     if necessary
    /// </summary>
    /// <param name="request">
    ///     Incoming request.
    /// </param>
    /// <param name="next">
    ///     Awaitable delegate for the next action in the pipeline.
    ///     Eventually this delegate represents the handler.
    /// </param>
    /// <param name="cancellationToken">
    ///     Cancellation token
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncEnumerable{T}" /> representing the stream of data.
    /// </returns>
    IAsyncEnumerable<TResponse> HandleAsync(TRequest request, StreamHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}