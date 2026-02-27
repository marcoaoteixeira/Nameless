// ReSharper disable SeparateLocalFunctionsWithJumpStatement

using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Mediator.Streams;

/// <summary>
///     Default implementation of <see cref="StreamHandlerWrapper{TResponse}" />.
/// </summary>
/// <typeparam name="TRequest">
///     Type fo the request.
/// </typeparam>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public class StreamHandlerWrapperImpl<TRequest, TResponse> : StreamHandlerWrapper<TResponse>
    where TRequest : IStream<TResponse> {
    /// <inheritdoc />
    public override async IAsyncEnumerable<object?> HandleAsync(object request, IServiceProvider provider,
        [EnumeratorCancellation] CancellationToken cancellationToken) {
        var stream = HandleAsync((IStream<TResponse>)request, provider, cancellationToken);

        await foreach (var item in stream) {
            yield return item;
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="provider"/> is <see langword="null"/>.
    /// </exception>
    public override async IAsyncEnumerable<TResponse> HandleAsync(IStream<TResponse> request, IServiceProvider provider,
        [EnumeratorCancellation] CancellationToken cancellationToken) {
        var items = provider.GetServices<IStreamPipelineBehavior<TRequest, TResponse>>()
                            .Reverse()
                            .Aggregate(
                                (StreamHandlerDelegate<TResponse>)InnerHandlerAsync,
                                (next, pipeline) => () => pipeline.HandleAsync((TRequest)request,
                                    () => NextWrapper(next(), cancellationToken), cancellationToken))
                            .Invoke();

        await foreach (var item in items.WithCancellation(cancellationToken)) {
            yield return item;
        }

        IAsyncEnumerable<TResponse> InnerHandlerAsync() {
            return provider.GetRequiredService<IStreamHandler<TRequest, TResponse>>()
                           .HandleAsync((TRequest)request, cancellationToken);
        }
    }

    private static async IAsyncEnumerable<T> NextWrapper<T>(IAsyncEnumerable<T> items, [EnumeratorCancellation] CancellationToken cancellationToken) {
        var stream = items.WithCancellation(cancellationToken);

        await foreach (var item in stream) {
            yield return item;
        }
    }
}