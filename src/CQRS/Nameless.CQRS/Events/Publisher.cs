using System.Collections.Concurrent;

namespace Nameless.CQRS.Events;

public class Publisher : IPublisher {
    private readonly ConcurrentDictionary<Type, EventHandlerWrapper> _cache = new();

    private readonly IServiceProvider _provider;

    public Publisher(IServiceProvider provider) {
        _provider = Prevent.Argument.Null(provider);
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IEvent {
        var handler = _cache.GetOrAdd(@event.GetType(), static eventType => {
            var wrapperType = typeof(EventHandlerWrapperImpl<>).MakeGenericType(eventType);
            var wrapper = Activator.CreateInstance(wrapperType)
                          ?? throw new InvalidOperationException($"Couldn't create event handler wrapper for event: {eventType}");

            return (EventHandlerWrapper)wrapper;
        });

        return handler.HandleAsync(@event, _provider, cancellationToken);
    }
}