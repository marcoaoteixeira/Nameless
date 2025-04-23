namespace Nameless.Mediator.Streams;

public abstract class StreamHandlerWrapperBase {
    public abstract IAsyncEnumerable<object?> HandleAsync(object request,
                                                          IServiceProvider serviceProvider,
                                                          CancellationToken cancellationToken);
}

public abstract class StreamHandlerWrapper<TResponse> : StreamHandlerWrapperBase {
    public abstract IAsyncEnumerable<TResponse> HandleAsync(IStream<TResponse> request,
                                                            IServiceProvider serviceProvider,
                                                            CancellationToken cancellationToken);
}