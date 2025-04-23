namespace Nameless.Mediator.Requests;

public abstract class RequestHandlerWrapperBase {
    public abstract Task<object?> HandleAsync(object request,
                                              IServiceProvider serviceProvider,
                                              CancellationToken cancellationToken);
}

public abstract class RequestHandlerWrapper : RequestHandlerWrapperBase {
    public abstract Task<Nothing> HandleAsync(IRequest request,
                                              IServiceProvider serviceProvider,
                                              CancellationToken cancellationToken);
}

public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerWrapperBase {
    public abstract Task<TResponse> HandleAsync(IRequest<TResponse> request,
                                                IServiceProvider serviceProvider,
                                                CancellationToken cancellationToken);
}