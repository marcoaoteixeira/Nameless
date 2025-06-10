using System.Collections.Concurrent;

namespace Nameless.Mediator.Streams;

/// <summary>
///     The default implementation of <see cref="IStreamHandlerInvoker" />.
/// </summary>
public sealed class StreamHandlerInvoker : IStreamHandlerInvoker {
    private readonly ConcurrentDictionary<Type, StreamHandlerWrapperBase> _cache = new();

    private readonly IServiceProvider _provider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StreamHandlerInvoker" /> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="provider"/> is <see langword="null"/>.
    /// </exception>
    public StreamHandlerInvoker(IServiceProvider provider) {
        _provider = Prevent.Argument.Null(provider);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="request"/> is <see langword="null"/>.
    /// </exception>
    public IAsyncEnumerable<TResponse> CreateAsync<TResponse>(IStream<TResponse> request,
                                                              CancellationToken cancellationToken) {
        Prevent.Argument.Null(request);

        var handler = _cache.GetOrAdd(request.GetType(), CreateStreamHandlerWrapper);

        return ((StreamHandlerWrapper<TResponse>)handler).HandleAsync(request, _provider, cancellationToken);

        static StreamHandlerWrapperBase CreateStreamHandlerWrapper(Type requestType) {
            var wrapperType = typeof(StreamHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType)
                       ?? throw new InvalidOperationException(
                              $"Couldn't create stream handler wrapper for request: {requestType}");
            return (StreamHandlerWrapperBase)wrapper;
        }
    }
}