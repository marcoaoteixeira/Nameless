using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Mediator.Requests;

/// <summary>
///     Default implementation of
///     <see cref="RequestHandlerWrapper{TResponse}" />.
/// </summary>
/// <typeparam name="TRequest">
///     Type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse> {
    /// <inheritdoc />
    public override async Task<object?> HandleAsync(object request, IServiceProvider provider,
        CancellationToken cancellationToken) {
        Guard.Against.Null(request);
        Guard.Against.Null(provider);

        return await HandleAsync((IRequest<TResponse>)request, provider, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
    }

    /// <inheritdoc />
    public override Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider provider,
        CancellationToken cancellationToken) {
        Guard.Against.Null(request);
        Guard.Against.Null(provider);

        return provider.GetServices<IRequestPipelineBehavior<TRequest, TResponse>>()
                       .Reverse()
                       .Aggregate(
                           (RequestHandlerDelegate<TResponse>)InnerHandleAsync,
                           (next, pipeline) => token => pipeline.HandleAsync((TRequest)request, next,
                               token == CancellationToken.None ? cancellationToken : token))
                       .Invoke(cancellationToken);

        Task<TResponse> InnerHandleAsync(CancellationToken token) {
            return provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
                           .HandleAsync((TRequest)request, token);
        }
    }
}