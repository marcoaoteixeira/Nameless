using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Patterns.Mediator.Events;

/// <summary>
/// The default implementation of <see cref="EventHandlerWrapper"/>.
/// </summary>
/// <typeparam name="TEvent">Type of the event.</typeparam>
public sealed class EventHandlerWrapperImpl<TEvent> : EventHandlerWrapper
    where TEvent : IEvent {
    /// <inheritdoc />
    public override Task HandleAsync(IEvent evt,
                                     IServiceProvider serviceProvider,
                                     CancellationToken cancellationToken) {
        Prevent.Argument.Null(evt);
        Prevent.Argument.Null(serviceProvider);

        var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>()
                                      .Select(handler => handler.HandleAsync((TEvent)evt, cancellationToken));

        return Task.WhenAll(handlers);
    }
}