using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Pipeline;

namespace Nameless.Mediator.Streams;

public class StreamHandlerWrapperImpl<TRequest, TResponse> : StreamHandlerWrapper<TResponse>
    where TRequest : IStream<TResponse> {
    public override async IAsyncEnumerable<object?> HandleAsync(object request,
                                                                IServiceProvider serviceProvider,
                                                                [EnumeratorCancellation]
                                                                CancellationToken cancellationToken) {
        await foreach (var item in HandleAsync((IStream<TResponse>)request, serviceProvider, cancellationToken)) {
            yield return item;
        }
    }

    public override async IAsyncEnumerable<TResponse> HandleAsync(IStream<TResponse> request,
                                                                  IServiceProvider serviceProvider,
                                                                  [EnumeratorCancellation]
                                                                  CancellationToken cancellationToken) {
        var items = serviceProvider.GetServices<IStreamPipelineBehavior<TRequest, TResponse>>()
                                   .Reverse()
                                   .Aggregate(seed: (StreamHandlerDelegate<TResponse>)InnerHandlerAsync,
                                              func: (next, pipeline) => () => pipeline.HandleAsync(request: (TRequest)request,
                                                                                                   next: () => NextWrapper(next(), cancellationToken),
                                                                                                   cancellationToken: cancellationToken))();

        await foreach (var item in items.WithCancellation(cancellationToken)) {
            yield return item;
        }

        yield break;

        IAsyncEnumerable<TResponse> InnerHandlerAsync() => serviceProvider.GetRequiredService<IStreamHandler<TRequest, TResponse>>()
                                                                          .HandleAsync((TRequest)request, cancellationToken);
    }

    private static async IAsyncEnumerable<T> NextWrapper<T>(IAsyncEnumerable<T> items,
                                                            [EnumeratorCancellation]
                                                            CancellationToken cancellationToken) {
        var cancellable = items.WithCancellation(cancellationToken)
                               .ConfigureAwait(continueOnCapturedContext: false);

        await foreach (var item in cancellable) {
            yield return item;
        }
    }
}