namespace Nameless.Mediator.Requests;

/// <summary>
///     Defines a request handler that returns a response.
/// </summary>
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