namespace Nameless.Mediator.Requests;

/// <summary>
///     Defines a request handler that returns a response.
/// </summary>
/// <remarks>
///     A request has exactly one handler. Dispatch uses the runtime type of
///     the request and the response type it declares, exactly: a handler for
///     a base request type does not handle derived requests.
/// </remarks>
/// <typeparam name="TRequest">
///     Type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    /// <summary>
    ///     Handles the request asynchronously.
    /// </summary>
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
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
///     Defines a request handler that does not return a response.
/// </summary>
/// <remarks>
///     Only for requests that do not declare a response
///     (<see cref="IRequest{TResponse}"/>); those are always dispatched to
///     <see cref="IRequestHandler{TRequest,TResponse}"/>.
/// </remarks>
/// <typeparam name="TRequest">
///     Type of the request.
/// </typeparam>
public interface IRequestHandler<in TRequest>
    where TRequest : IRequest {
    /// <summary>
    ///     Handles the request asynchronously.
    /// </summary>
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
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}
