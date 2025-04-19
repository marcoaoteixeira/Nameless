using System.Collections.Concurrent;

namespace Nameless.CQRS.Requests;

public sealed class Requester : IRequester {
    private readonly IServiceProvider _provider;

    private readonly ConcurrentDictionary<Type, RequestHandlerWrapperBase> _cache = new();

    public Requester(IServiceProvider provider) {
        _provider = Prevent.Argument.Null(provider);
    }

    public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest  {
        Prevent.Argument.Default(request);

        var handler = _cache.GetOrAdd(request.GetType(), static requestType => {
            var wrapperType = typeof(RequestHandlerWrapperImpl<>).MakeGenericType(requestType);
            var wrapper = Activator.CreateInstance(wrapperType)
                          ?? throw new InvalidOperationException($"Couldn't create request handler wrapper for request: {requestType}");

            return (RequestHandlerWrapperBase)wrapper;
        });

        return ((RequestHandlerWrapper)handler).HandleAsync(request, _provider, cancellationToken);
    }

    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) {
        Prevent.Argument.Null(request);

        var handler = _cache.GetOrAdd(request.GetType(), static requestType => {
            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType)
                          ?? throw new InvalidOperationException($"Couldn't create wrapper for request: {requestType}");

            return (RequestHandlerWrapperBase)wrapper;
        });

        return ((RequestHandlerWrapper<TResponse>)handler).HandleAsync(request, _provider, cancellationToken);
    }
}