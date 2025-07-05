using System.Collections.Concurrent;

namespace Nameless.Mediator.Requests;

/// <summary>
///     The default implementation of <see cref="IRequestHandlerInvoker" />.
/// </summary>
public sealed class RequestHandlerInvoker : IRequestHandlerInvoker {
    private readonly ConcurrentDictionary<Type, RequestHandlerWrapperBase> _cache = new();
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RequestHandlerInvoker" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service serviceProvider.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="serviceProvider"/> is <see langword="null"/>.
    /// </exception>
    public RequestHandlerInvoker(IServiceProvider serviceProvider) {
        _serviceProvider = Prevent.Argument.Null(serviceProvider);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="request"/> is <see langword="null"/>.
    /// </exception>
    public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest {
        Prevent.Argument.Default(request);

        var handler = _cache.GetOrAdd(request.GetType(), CreateRequestHandlerWrapper);

        return ((RequestHandlerWrapper)handler).HandleAsync(request, _serviceProvider, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) {
        Prevent.Argument.Null(request);

        var handler = _cache.GetOrAdd(request.GetType(), CreateRequestHandlerWrapper<TResponse>);

        return ((RequestHandlerWrapper<TResponse>)handler).HandleAsync(request, _serviceProvider, cancellationToken);
    }

    private static RequestHandlerWrapperBase CreateRequestHandlerWrapper(Type requestType) {
        var wrapperType = typeof(RequestHandlerWrapperImpl<>).MakeGenericType(requestType);

        return CreateRequestHandlerWrapperCore(wrapperType, requestType);
    }

    private static RequestHandlerWrapperBase CreateRequestHandlerWrapper<TResponse>(Type requestType) {
        var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));

        return CreateRequestHandlerWrapperCore(wrapperType, requestType);
    }

    private static RequestHandlerWrapperBase CreateRequestHandlerWrapperCore(Type wrapperType, Type requestType) {
        var wrapper = Activator.CreateInstance(wrapperType)
                   ?? throw new InvalidOperationException($"Couldn't create request handler wrapper for request: {requestType}");

        return (RequestHandlerWrapperBase)wrapper;
    }
}