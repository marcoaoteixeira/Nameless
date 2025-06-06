namespace Nameless.Mediator.Requests;

/// <summary>
///     The base interface for all request handlers.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
public interface IRequestHandler<in TRequest>
    where TRequest : IRequest {
    /// <summary>
    ///     Handles the request asynchronously.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task" /> representing the request execution.</returns>
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
///     The base interface for all request handlers.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    /// <summary>
    ///     Handles the request asynchronously.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task{T}" /> representing the request execution.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}