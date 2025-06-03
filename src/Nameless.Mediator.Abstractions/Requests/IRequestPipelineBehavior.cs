﻿namespace Nameless.Mediator.Requests;

/// <summary>
///     Pipeline behavior to surround the inner handler.
///     Implementations add additional behavior and await the next delegate.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IRequestPipelineBehavior<in TRequest, TResponse>
    where TRequest : notnull {
    /// <summary>
    ///     Pipeline handler. Perform any additional behavior and await the <paramref name="next" /> delegate as necessary
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">
    ///     Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the
    ///     handler.
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse" /></returns>
    Task<TResponse> HandleAsync(TRequest request,
                                RequestHandlerDelegate<TResponse> next,
                                CancellationToken cancellationToken);
}