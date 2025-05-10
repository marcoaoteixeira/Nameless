using System.Collections.Concurrent;

namespace Nameless.Patterns.Mediator.Events;

/// <summary>
/// The default implementation of <see cref="IEventHandlerProxy"/>.
/// </summary>
public sealed class EventHandlerProxy : IEventHandlerProxy {
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
    public Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IEvent {
        var handler = _cache.GetOrAdd(typeof(TEvent), CreateEventHandlerWrapper);

        return handler.HandleAsync(evt, _provider, cancellationToken);
    }

    private static EventHandlerWrapper CreateEventHandlerWrapper(Type evtType) {
        var wrapperType = typeof(EventHandlerWrapperImpl<>).MakeGenericType(evtType);
        var wrapper = Activator.CreateInstance(wrapperType)
                      ?? throw new InvalidOperationException($"Couldn't create event handler wrapper for event: {evtType}");

        return (EventHandlerWrapper)wrapper;
    }   
}