using Nameless.Patterns.Mediator.Events;
using Nameless.Patterns.Mediator.Requests;
using Nameless.Patterns.Mediator.Streams;

namespace Nameless.Patterns.Mediator;

public sealed class MediatorImpl : IMediator {
    private readonly IRequestHandlerProxy _requestHandlerProxy;
    private readonly IEventHandlerProxy _eventHandlerProxy;
    private readonly IStreamHandlerProxy _streamHandlerProxy;

    public MediatorImpl(IRequestHandlerProxy requestHandlerProxy,
                        IEventHandlerProxy eventHandlerProxy,
                        IStreamHandlerProxy streamHandlerProxy) {
        _requestHandlerProxy = Prevent.Argument.Null(requestHandlerProxy);
        _eventHandlerProxy = Prevent.Argument.Null(eventHandlerProxy);
        _streamHandlerProxy = Prevent.Argument.Null(streamHandlerProxy);
    }

    public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest
        => _requestHandlerProxy.ExecuteAsync(request, cancellationToken);

    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        => _requestHandlerProxy.ExecuteAsync(request, cancellationToken);

    public Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken) where TEvent : IEvent
        => _eventHandlerProxy.PublishAsync(evt, cancellationToken);

    public IAsyncEnumerable<TResponse> CreateAsync<TResponse>(IStream<TResponse> request, CancellationToken cancellationToken)
        => _streamHandlerProxy.CreateAsync(request, cancellationToken);
}