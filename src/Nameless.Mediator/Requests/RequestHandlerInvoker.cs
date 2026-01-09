using System.Collections.Concurrent;

namespace Nameless.Mediator.Requests;

/// <summary>
///     The default implementation of <see cref="IRequestHandlerInvoker" />.
/// </summary>
public class RequestHandlerInvoker : IRequestHandlerInvoker {
    private readonly ConcurrentDictionary<Type, RequestHandlerWrapper> _cache = new();
    private readonly IServiceProvider _provider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RequestHandlerInvoker" /> class.
    /// </summary>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="provider"/> is <see langword="null"/>.
    /// </exception>
    public RequestHandlerInvoker(IServiceProvider provider) {
        _provider = provider;
    }

    /// <inheritdoc />
    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) {
        Guard.Against.Null(request);

        var handler = _cache.GetOrAdd(request.GetType(), CreateRequestHandlerWrapper);

        return ((RequestHandlerWrapper<TResponse>)handler).HandleAsync(request, _provider, cancellationToken);

        static RequestHandlerWrapper CreateRequestHandlerWrapper(Type requestType) {
            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType)
                          ?? throw new InvalidOperationException(
                              $"Couldn't create request handler wrapper for request '{requestType.GetPrettyName()}'.");

            return (RequestHandlerWrapper)wrapper;
        }
    }
}