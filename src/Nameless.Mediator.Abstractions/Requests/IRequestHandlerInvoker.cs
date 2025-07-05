namespace Nameless.Mediator.Requests;

/// <summary>
///     An interface that represents a request handler invoker.
/// </summary>
public interface IRequestHandlerInvoker {
    /// <summary>
    ///     Executes a request asynchronously.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task" /> represeting the request execution.</returns>
    Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest;

    /// <summary>
    ///     Executes a request asynchronously.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task{T}" /> represeting the request execution.</returns>
    Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
}