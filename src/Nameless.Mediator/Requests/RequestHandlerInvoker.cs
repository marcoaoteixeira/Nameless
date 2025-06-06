using System.Collections.Concurrent;

namespace Nameless.Mediator.Requests;

/// <summary>
///     The default implementation of <see cref="IRequestHandlerInvoker" />.
/// </summary>
public sealed class RequestHandlerInvoker : IRequestHandlerInvoker {
    private readonly ConcurrentDictionary<Type, RequestHandlerWrapperBase> _cache = new();
    private readonly IServiceProvider _provider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RequestHandlerInvoker" /> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="provider"/> is <c>null</c>.
    /// </exception>
    public RequestHandlerInvoker(IServiceProvider provider) {
        _provider = Prevent.Argument.Null(provider);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="request"/> is <c>null</c>.
    /// </exception>
    public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest {
        Prevent.Argument.Default(request);

        var handler = _cache.GetOrAdd(request.GetType(), static requestType => {
            var wrapperType = typeof(RequestHandlerWrapperImpl<>).MakeGenericType(requestType);
            var wrapper = Activator.CreateInstance(wrapperType)
                       ?? throw new InvalidOperationException(
                              $"Couldn't create request handler wrapper for request: {requestType}");

            return (RequestHandlerWrapperBase)wrapper;
        });

        return ((RequestHandlerWrapper)handler).HandleAsync(request, _provider, cancellationToken);
    }

    /// <inheritdoc />
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