using System.Collections.Concurrent;

namespace Nameless.CQRS.Streams;

public sealed class Streamer : IStreamer {
    private readonly ConcurrentDictionary<Type, StreamHandlerWrapper> _cache = new();

    private readonly IServiceProvider _provider;

    public Streamer(IServiceProvider provider) {
        _provider = Prevent.Argument.Null(provider);
    }

    public IAsyncEnumerable<TResponse> StreamAsync<TResponse>(IStreamRequest<TResponse> request,
                                                              CancellationToken cancellationToken) {
        Prevent.Argument.Null(request);

        var handler = _cache.GetOrAdd(request.GetType(), static requestType => {
            var wrapperType = typeof(StreamHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType)
                          ?? throw new InvalidOperationException($"Couldn't create stream handler wrapper for request: {requestType}");
            return (StreamHandlerWrapper)wrapper;
        });

        return ((StreamHandlerWrapper<TResponse>)handler).HandleAsync(request,
                                                                      _provider,
                                                                      cancellationToken);
    }
}
