namespace Nameless.Mediator.Requests;

/// <summary>
///     Defines a pipeline behavior to surround the request handler and
///     adds additional behavior.
/// </summary>
/// <typeparam name="TRequest">
///     Type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public interface IRequestPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    /// <summary>
    ///     Executes the pipeline handler performing any additional
    ///     behavior, then executes the <paramref name="next" /> delegate,
    ///     if necessary
    /// </summary>
    /// <param name="request">
    ///     Incoming request
    /// </param>
    /// <param name="next">
    ///     Awaitable delegate for the next action in the pipeline.
    ///     Eventually this delegate represents the handler.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the action
    ///     asynchronous operation, where <typeparamref name="TResponse"/>
    ///     is the task result.
    /// </returns>
    Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}

/// <summary>
///     Defines a pipeline behavior to surround the handler of a request
///     that does not return a response and adds additional behavior.
/// </summary>
/// <typeparam name="TRequest">
///     Type of the request.
/// </typeparam>
public interface IRequestPipelineBehavior<in TRequest>
    where TRequest : IRequest {
    /// <summary>
    ///     Executes the pipeline handler performing any additional
    ///     behavior, then executes the <paramref name="next" /> delegate,
    ///     if necessary
    /// </summary>
    /// <param name="request">
    ///     Incoming request
    /// </param>
    /// <param name="next">
    ///     Awaitable delegate for the next action in the pipeline.
    ///     Eventually this delegate represents the handler.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing the action asynchronous
    ///     operation.
    /// </returns>
    Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken);
}
