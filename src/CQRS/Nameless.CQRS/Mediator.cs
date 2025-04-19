using Nameless.CQRS.Events;
using Nameless.CQRS.Requests;
using Nameless.CQRS.Streams;

namespace Nameless.CQRS;

public sealed class Mediator : IMediator {
    private readonly IRequester _requester;
    private readonly IPublisher _publisher;
    private readonly IStreamer _streamer;

    public Mediator(IRequester requester, IPublisher publisher, IStreamer streamer) {
        _requester = Prevent.Argument.Null(requester);
        _publisher = Prevent.Argument.Null(publisher);
        _streamer = Prevent.Argument.Null(streamer);
    }

    public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest
        => _requester.ExecuteAsync(request, cancellationToken);

    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        => _requester.ExecuteAsync(request, cancellationToken);

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
        => _publisher.PublishAsync(@event, cancellationToken);

    public IAsyncEnumerable<TResponse> StreamAsync<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken)
        => _streamer.StreamAsync(request, cancellationToken);
}