using System.Collections.Concurrent;

namespace Nameless.Mediator.Events;

/// <summary>
/// The default implementation of <see cref="IEventHandlerProxy"/>.
/// </summary>
public class EventHandlerProxy : IEventHandlerProxy {
    private readonly ConcurrentDictionary<Type, EventHandlerWrapper> _cache = new();

    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerProxy"/>.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public EventHandlerProxy(IServiceProvider provider) {
        _provider = Prevent.Argument.Null(provider);
    }

    /// <inheritdoc />
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