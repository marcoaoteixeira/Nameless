// ReSharper disable SeparateLocalFunctionsWithJumpStatement
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Mediator.Streams;

/// <summary>
///     Default implementation of <see cref="StreamHandlerWrapper{TResponse}" />.
/// </summary>
/// <typeparam name="TRequest">Type fo the request. Must implement <see cref="IStream{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public class StreamHandlerWrapperImpl<TRequest, TResponse> : StreamHandlerWrapper<TResponse>
    where TRequest : IStream<TResponse> {
    /// <inheritdoc />
    public override async IAsyncEnumerable<object?> HandleAsync(object request,
                                                                IServiceProvider serviceProvider,
                                                                [EnumeratorCancellation]
                                                                CancellationToken cancellationToken) {
        await foreach (var item in HandleAsync((IStream<TResponse>)request, serviceProvider, cancellationToken)) {
            yield return item;
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="serviceProvider"/> is <c>null</c>.
    /// </exception>
    public override async IAsyncEnumerable<TResponse> HandleAsync(IStream<TResponse> request,
                                                                  IServiceProvider serviceProvider,
                                                                  [EnumeratorCancellation]
                                                                  CancellationToken cancellationToken) {
        var items = serviceProvider.GetServices<IStreamPipelineBehavior<TRequest, TResponse>>()
                                   .Reverse()
                                   .Aggregate((StreamHandlerDelegate<TResponse>)InnerHandlerAsync,
                                        (next, pipeline) => () => pipeline.HandleAsync(
                                            request: (TRequest)request,
                                            next: () => NextWrapper(next(), cancellationToken),
                                            cancellationToken: cancellationToken)
                                        ).Invoke();

        await foreach (var item in items.WithCancellation(cancellationToken)) {
            yield return item;
        }

        IAsyncEnumerable<TResponse> InnerHandlerAsync() {
            return serviceProvider.GetRequiredService<IStreamHandler<TRequest, TResponse>>()
                                  .HandleAsync((TRequest)request, cancellationToken);
        }
    }

    private static async IAsyncEnumerable<T> NextWrapper<T>(IAsyncEnumerable<T> items,
                                                            [EnumeratorCancellation]
                                                            CancellationToken cancellationToken) {
        var cancellable = items.WithCancellation(cancellationToken)
                               .ConfigureAwait(false);

        await foreach (var item in cancellable) {
            yield return item;
        }
    }
}