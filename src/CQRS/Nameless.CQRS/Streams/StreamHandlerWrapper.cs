namespace Nameless.CQRS.Streams;

public abstract class StreamHandlerWrapper {
    public abstract IAsyncEnumerable<object?> HandleAsync(object request,
                                                          IServiceProvider serviceProvider,
                                                          CancellationToken cancellationToken);
}

public abstract class StreamHandlerWrapper<TResponse> : StreamHandlerWrapper {
    public abstract IAsyncEnumerable<TResponse> HandleAsync(IStreamRequest<TResponse> request,
                                                            IServiceProvider serviceProvider,
                                                            CancellationToken cancellationToken);
}