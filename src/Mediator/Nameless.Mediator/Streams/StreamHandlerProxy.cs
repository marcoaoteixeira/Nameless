using System.Collections.Concurrent;

namespace Nameless.Mediator.Streams;

/// <summary>
/// The default implementation of <see cref="IStreamHandlerProxy"/>.
/// </summary>
public sealed class StreamHandlerProxy : IStreamHandlerProxy {
    private readonly ConcurrentDictionary<Type, StreamHandlerWrapperBase> _cache = new();

    private readonly IServiceProvider _provider;

    public StreamHandlerProxy(IServiceProvider provider) {
        _provider = Prevent.Argument.Null(provider);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TResponse> CreateAsync<TResponse>(IStream<TResponse> request,
                                                              CancellationToken cancellationToken) {
        Prevent.Argument.Null(request);

        var handler = _cache.GetOrAdd(request.GetType(), static requestType => {
            var wrapperType = typeof(StreamHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType)
                          ?? throw new InvalidOperationException($"Couldn't create stream handler wrapper for request: {requestType}");
            return (StreamHandlerWrapperBase)wrapper;
        });

        return ((StreamHandlerWrapper<TResponse>)handler).HandleAsync(request,
                                                                      _provider,
                                                                      cancellationToken);
    }
}
