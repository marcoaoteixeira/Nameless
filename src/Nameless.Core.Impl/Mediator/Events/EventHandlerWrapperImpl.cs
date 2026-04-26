using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Mediator.Events;

/// <summary>
///     The default implementation of <see cref="EventHandlerWrapper" />.
/// </summary>
/// <typeparam name="TEvent">
///     Type of the event.
/// </typeparam>
public class EventHandlerWrapperImpl<TEvent> : EventHandlerWrapper
    where TEvent : IEvent {
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="evt"/> or
    ///     <paramref name="provider"/>is <see langword="null"/>.
    /// </exception>
    public override Task HandleAsync(IEvent evt, IServiceProvider provider, CancellationToken cancellationToken) {
        var logger = provider.GetLogger<EventHandlerWrapper>();
        var handlers = provider.GetServices<IEventHandler<TEvent>>().ToArray();

        if (handlers.Length == 0) {
            logger.MissingEventHandler(evt);
        }

        var executions = handlers.Select(
            handler => handler.HandleAsync((TEvent)evt, cancellationToken)
        );

        return Task.WhenAll(executions);
    }
}