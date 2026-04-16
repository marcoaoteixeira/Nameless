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
}