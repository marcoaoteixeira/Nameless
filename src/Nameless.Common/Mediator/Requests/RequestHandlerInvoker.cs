using System.Collections.Concurrent;

namespace Nameless.Mediator.Requests;

/// <summary>
///     Default implementation of <see cref="IRequestHandlerInvoker"/>.
/// </summary>
public class RequestHandlerInvoker : IRequestHandlerInvoker {
    private readonly ConcurrentDictionary<Type, RequestHandlerWrapper> _cache = new();
    private readonly IServiceProvider _provider;

    /// <summary>
    ///     Initializes a new instance of <see cref="RequestHandlerInvoker"/>.
    /// </summary>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    public RequestHandlerInvoker(IServiceProvider provider) {
        _provider = provider;
    }

    /// <inheritdoc />
    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) {
        Throws.When.Null(request);

        var handler = _cache.GetOrAdd(request.GetType(), CreateRequestHandlerWrapper);

        return ((RequestHandlerWrapper<TResponse>)handler).HandleAsync(request, _provider, cancellationToken);
    }

    /// <inheritdoc />
    public Task ExecuteAsync(IRequest request, CancellationToken cancellationToken) {
        Throws.When.Null(request);

        var handler = _cache.GetOrAdd(request.GetType(), CreateRequestHandlerWrapper);

        return handler.HandleAsync(request, _provider, cancellationToken);
    }

    // A single factory keyed by the request runtime type guarantees that a
    // request type maps to exactly one wrapper, whichever overload creates
    // it first.
    private static RequestHandlerWrapper CreateRequestHandlerWrapper(Type requestType) {
        var responseTypes = requestType.GetInterfacesThatCloses(typeof(IRequest<>))
                                       .Select(type => type.GetGenericArguments()[0])
                                       .ToArray();

        var wrapperType = responseTypes.Length switch {
            0 => typeof(RequestHandlerWrapperImpl<>).MakeGenericType(requestType),
            1 => typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseTypes[0]),
            _ => throw new InvalidOperationException(
                $"Request '{requestType.GetPrettyName()}' implements more than one '{typeof(IRequest<>).Name}'; unable to determine its response type."
            )
        };

        var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException(
            $"Couldn't create request handler wrapper for request '{requestType.GetPrettyName()}'."
        );

        return (RequestHandlerWrapper)wrapper;
    }
}
