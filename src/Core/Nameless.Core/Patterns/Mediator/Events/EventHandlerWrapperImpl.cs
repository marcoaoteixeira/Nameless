using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Patterns.Mediator.Events;

public sealed class EventHandlerWrapperImpl<TEvent> : EventHandlerWrapper
    where TEvent : IEvent {
    public override Task HandleAsync(IEvent @event,
                                     IServiceProvider serviceProvider,
                                     CancellationToken cancellationToken) {
        Prevent.Argument.Null(@event);
        Prevent.Argument.Null(serviceProvider);

        var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>()
                                      .Select(handler => handler.HandleAsync((TEvent)@event, cancellationToken));

        return Task.WhenAll(handlers);
    }
}