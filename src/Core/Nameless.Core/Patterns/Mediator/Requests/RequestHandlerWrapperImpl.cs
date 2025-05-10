using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Patterns.Mediator.Requests;

/// <summary>
/// Default implementation of <see cref="RequestHandlerWrapper"/>.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
public sealed class RequestHandlerWrapperImpl<TRequest> : RequestHandlerWrapper
    where TRequest : IRequest {
    /// <inheritdoc />
    public override async Task<object?> HandleAsync(object request,
                                                    IServiceProvider serviceProvider,
                                                    CancellationToken cancellationToken)
        => await HandleAsync((TRequest)request,
                             serviceProvider,
                             cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

    /// <inheritdoc />
    public override Task<Nothing> HandleAsync(IRequest request,
                                              IServiceProvider serviceProvider,
                                              CancellationToken cancellationToken) {
        return serviceProvider.GetServices<IRequestPipelineBehavior<TRequest, Nothing>>()
                              .Reverse()
                              .Aggregate(seed: (RequestHandlerDelegate<Nothing>)InnerHandleAsync,
                                         func: (next, pipeline) => token => pipeline.HandleAsync((TRequest)request, next, token))(cancellationToken);

        async Task<Nothing> InnerHandleAsync(CancellationToken token) {
            await serviceProvider.GetRequiredService<IRequestHandler<TRequest>>()
                                 .HandleAsync((TRequest)request, token)
                                 .ConfigureAwait(continueOnCapturedContext: false);

            return Nothing.Value;
        }
    }
}

/// <summary>
/// Default implementation of <see cref="RequestHandlerWrapper"/>.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse> {
    /// <inheritdoc />
    public override async Task<object?> HandleAsync(object request,
                                                    IServiceProvider serviceProvider,
                                                    CancellationToken cancellationToken)
        => await HandleAsync((IRequest<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

    /// <inheritdoc />
    public override Task<TResponse> HandleAsync(IRequest<TResponse> request,
                                                IServiceProvider serviceProvider,
                                                CancellationToken cancellationToken) {
        return serviceProvider.GetServices<IRequestPipelineBehavior<TRequest, TResponse>>()
                              .Reverse()
                              .Aggregate(seed: (RequestHandlerDelegate<TResponse>)InnerHandleAsync,
                                         func: (next, pipeline) => token => pipeline.HandleAsync((TRequest)request, next, token))(cancellationToken);

        Task<TResponse> InnerHandleAsync(CancellationToken token) {
            return serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
                                  .HandleAsync((TRequest)request, token);
        }
    }
}