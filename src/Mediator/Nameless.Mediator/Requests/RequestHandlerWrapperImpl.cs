using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Pipeline;

namespace Nameless.Mediator.Requests;

public sealed class RequestHandlerWrapperImpl<TRequest> : RequestHandlerWrapper
    where TRequest : IRequest {
    public override async Task<object?> HandleAsync(object request,
                                                    IServiceProvider serviceProvider,
                                                    CancellationToken cancellationToken)
        => await HandleAsync((TRequest)request,
                             serviceProvider,
                             cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

    public override Task<Nothing> HandleAsync(IRequest request,
                                              IServiceProvider serviceProvider,
                                              CancellationToken cancellationToken) {
        return serviceProvider.GetServices<IPipelineBehavior<TRequest, Nothing>>()
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

public sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse> {
    public override async Task<object?> HandleAsync(object request,
                                                    IServiceProvider serviceProvider,
                                                    CancellationToken cancellationToken)
        => await HandleAsync((IRequest<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

    public override Task<TResponse> HandleAsync(IRequest<TResponse> request,
                                                IServiceProvider serviceProvider,
                                                CancellationToken cancellationToken) {
        return serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>()
                              .Reverse()
                              .Aggregate(seed: (RequestHandlerDelegate<TResponse>)InnerHandleAsync,
                                         func: (next, pipeline) => token => pipeline.HandleAsync((TRequest)request, next, token))(cancellationToken);

        Task<TResponse> InnerHandleAsync(CancellationToken token)
            => serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
                              .HandleAsync((TRequest)request, token);
    }
}