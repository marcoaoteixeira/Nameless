using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

/// <summary>
/// Default implementation of the <see cref="IMediator"/> interface.
/// </summary>
public sealed class MediatorImpl : IMediator {
    private readonly IEventHandlerInvoker _eventHandlerInvoker;
    private readonly IRequestHandlerInvoker _requestHandlerInvoker;
    private readonly IStreamHandlerInvoker _streamHandlerInvoker;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatorImpl"/> class.
    /// </summary>
    /// <param name="eventHandlerInvoker">The event handler invoker.</param>
    /// <param name="requestHandlerInvoker">The request handler invoker.</param>
    /// <param name="streamHandlerInvoker">The stream handler invoker.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="eventHandlerInvoker"/> or
    ///     <paramref name="requestHandlerInvoker"/> or
    ///     <paramref name="streamHandlerInvoker"/> is <see langword="null"/>.
    /// </exception>
    public MediatorImpl(IEventHandlerInvoker eventHandlerInvoker,
        IRequestHandlerInvoker requestHandlerInvoker,
        IStreamHandlerInvoker streamHandlerInvoker) {
        _eventHandlerInvoker = Guard.Against.Null(eventHandlerInvoker);
        _requestHandlerInvoker = Guard.Against.Null(requestHandlerInvoker);
        _streamHandlerInvoker = Guard.Against.Null(streamHandlerInvoker);
    }

    /// <inheritdoc />
    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) {
        return _requestHandlerInvoker.ExecuteAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IEvent {
        return _eventHandlerInvoker.PublishAsync(evt, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TResponse> CreateAsync<TResponse>(IStream<TResponse> request,
        CancellationToken cancellationToken) {
        return _streamHandlerInvoker.CreateAsync(request, cancellationToken);
    }
}