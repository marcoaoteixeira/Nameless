using System.Collections.Concurrent;

namespace Nameless.Mediator.Events;

/// <summary>
///     The default implementation of <see cref="IEventHandlerInvoker" />.
/// </summary>
internal sealed class EventHandlerInvoker : IEventHandlerInvoker {
    // Wrappers are stateless and keyed by type, so they are shared by every
    // invoker instance (the invoker itself is transient).
    private static readonly ConcurrentDictionary<Type, EventHandlerWrapper> Cache = new();

    private readonly IServiceProvider _provider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventHandlerInvoker" />.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="provider"/> is <see langword="null"/>.
    /// </exception>
    public EventHandlerInvoker(IServiceProvider provider) {
        _provider = provider;
    }

    /// <inheritdoc />
    public Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IEvent {
        Throws.When.Null(evt);

        var handler = Cache.GetOrAdd(evt.GetType(), CreateEventHandlerWrapper);

        return handler.HandleAsync(evt, _provider, cancellationToken);
    }

    private static EventHandlerWrapper CreateEventHandlerWrapper(Type evtType) {
        var wrapperType = typeof(EventHandlerWrapperImpl<>).MakeGenericType(evtType);
        var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException(
            $"Couldn't create event handler wrapper for event '{evtType.GetPrettyName()}'."
        );

        return (EventHandlerWrapper)wrapper;
    }
}