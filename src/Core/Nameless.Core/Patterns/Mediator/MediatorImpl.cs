using Nameless.Patterns.Mediator.Events;
using Nameless.Patterns.Mediator.Requests;
using Nameless.Patterns.Mediator.Streams;

namespace Nameless.Patterns.Mediator;

public sealed class MediatorImpl : IMediator {
    private readonly IEventHandlerInvoker _eventHandlerInvoker;
    private readonly IRequestHandlerInvoker _requestHandlerInvoker;
    private readonly IStreamHandlerInvoker _streamHandlerInvoker;

    public MediatorImpl(IEventHandlerInvoker eventHandlerInvoker,
                        IRequestHandlerInvoker requestHandlerInvoker,
                        IStreamHandlerInvoker streamHandlerInvoker) {
        _eventHandlerInvoker = Prevent.Argument.Null(eventHandlerInvoker);
        _requestHandlerInvoker = Prevent.Argument.Null(requestHandlerInvoker);
        _streamHandlerInvoker = Prevent.Argument.Null(streamHandlerInvoker);
    }

    public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest
        => _requestHandlerInvoker.ExecuteAsync(request, cancellationToken);

    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        => _requestHandlerInvoker.ExecuteAsync(request, cancellationToken);

    public Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken) where TEvent : IEvent
        => _eventHandlerInvoker.PublishAsync(evt, cancellationToken);

    public IAsyncEnumerable<TResponse> CreateAsync<TResponse>(IStream<TResponse> request, CancellationToken cancellationToken)
        => _streamHandlerInvoker.CreateAsync(request, cancellationToken);
}