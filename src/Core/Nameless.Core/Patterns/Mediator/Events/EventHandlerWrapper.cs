namespace Nameless.Patterns.Mediator.Events;

public abstract class EventHandlerWrapper {
    public abstract Task HandleAsync(IEvent @event,
                                     IServiceProvider serviceProvider,
                                     CancellationToken cancellationToken);
}