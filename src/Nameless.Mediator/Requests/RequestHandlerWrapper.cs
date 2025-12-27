#pragma warning disable S1694

namespace Nameless.Mediator.Requests;

/// <summary>
///     A wrapper class for a request handler.
/// </summary>
public abstract class RequestHandlerWrapper {
    /// <summary>
    ///     Handles the request.
    /// </summary>
    /// <param name="request">
    ///     The request to handle.
    /// </param>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the action
    ///     asynchronous operation, where the task result contains an object
    ///     that represents the response.
    /// </returns>
    public abstract Task<object?> HandleAsync(object request, IServiceProvider provider,
        CancellationToken cancellationToken);
}

/// <summary>
///     A wrapper class for a request handler.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerWrapper {
    /// <summary>
    ///     Handles the request.
    /// </summary>
    /// <typeparam name="TResponse">
    ///     Type of the response.
    /// </typeparam>
    /// <param name="request">
    ///     The request to handle.
    /// </param>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the action
    ///     asynchronous operation, where the task result contains an object
    ///     of type <typeparamref name="TResponse" /> that represents the
    ///     response.
    /// </returns>
    public abstract Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider provider,
        CancellationToken cancellationToken);
}