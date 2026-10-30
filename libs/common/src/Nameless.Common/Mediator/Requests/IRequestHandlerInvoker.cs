namespace Nameless.Mediator.Requests;

/// <summary>
///     Defines a request handler invoker.
/// </summary>
public interface IRequestHandlerInvoker {
    /// <summary>
    ///     Executes a request asynchronously.
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
    ///     A <see cref="Task{TResult}" /> representing the action
    ///     asynchronous operation, where <typeparamref name="TResponse"/>
    ///     is the task result.
    /// </returns>
    Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);

    /// <summary>
    ///     Executes a request that does not return a response
    ///     asynchronously.
    /// </summary>
    /// <remarks>
    ///     If the runtime type of <paramref name="request"/> implements
    ///     <see cref="IRequest{TResponse}"/>, its handler is executed and
    ///     the response is discarded.
    /// </remarks>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing the action asynchronous
    ///     operation.
    /// </returns>
    Task ExecuteAsync(IRequest request, CancellationToken cancellationToken);
}
