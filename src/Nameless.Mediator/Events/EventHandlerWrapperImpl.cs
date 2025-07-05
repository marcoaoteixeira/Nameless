using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Mediator.Events;

/// <summary>
///     The default implementation of <see cref="EventHandlerWrapper" />.
/// </summary>
/// <typeparam name="TEvent">Type of the event.</typeparam>
public sealed class EventHandlerWrapperImpl<TEvent> : EventHandlerWrapper
    where TEvent : IEvent {
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="serviceProvider"/> is <see langword="null"/>.
    /// </exception>
    public override Task HandleAsync(IEvent evt,
                                     IServiceProvider serviceProvider,
                                     CancellationToken cancellationToken) {
        Prevent.Argument.Null(serviceProvider);

        var logger = serviceProvider.GetLogger<EventHandlerWrapper>();
        var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>()
                                      .ToArray();

        if (handlers.Length == 0) {
            logger.MissingEventHandler(evt);
        }

        var tasks = handlers.Select(handler => handler.HandleAsync((TEvent)evt, cancellationToken));

        return Task.WhenAll(tasks);
    }
}