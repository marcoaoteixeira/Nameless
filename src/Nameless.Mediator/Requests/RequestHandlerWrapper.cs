#pragma warning disable S1694

namespace Nameless.Mediator.Requests;

/// <summary>
///     A wrapper class for a request handler.
/// </summary>
public abstract class RequestHandlerWrapperBase {
    public abstract Task<object?> HandleAsync(object request,
                                              IServiceProvider serviceProvider,
                                              CancellationToken cancellationToken);
}

/// <summary>
///     A wrapper class for a request handler.
/// </summary>
public abstract class RequestHandlerWrapper : RequestHandlerWrapperBase {
    public abstract Task<Nothing> HandleAsync(IRequest request,
                                              IServiceProvider serviceProvider,
                                              CancellationToken cancellationToken);
}

/// <summary>
///     A wrapper class for a request handler.
/// </summary>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerWrapperBase {
    public abstract Task<TResponse> HandleAsync(IRequest<TResponse> request,
                                                IServiceProvider serviceProvider,
                                                CancellationToken cancellationToken);
}