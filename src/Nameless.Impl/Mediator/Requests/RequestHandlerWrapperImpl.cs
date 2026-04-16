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
public class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse> {
    /// <inheritdoc />
    public override async Task<object?> HandleAsync(object request, IServiceProvider provider, CancellationToken cancellationToken) {
        return await HandleAsync((IRequest<TResponse>)request, provider, cancellationToken).SkipContextSync();
    }

    /// <inheritdoc />
    public override Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider provider, CancellationToken cancellationToken) {
        return provider.GetServices<IRequestPipelineBehavior<TRequest, TResponse>>()
                       .Reverse()
                       .Aggregate(
                           seed: (RequestHandlerDelegate<TResponse>)InnerHandleAsync,
                           func: (next, pipeline) => token => pipeline.HandleAsync(
                               request: (TRequest)request,
                               next: next,
                               cancellationToken: token == CancellationToken.None ? cancellationToken : token
                            )
                        )
                       .Invoke(cancellationToken);

        Task<TResponse> InnerHandleAsync(CancellationToken token) {
            return provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
                           .HandleAsync((TRequest)request, token);
        }
    }
}