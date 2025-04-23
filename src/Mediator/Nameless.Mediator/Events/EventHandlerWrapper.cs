namespace Nameless.Mediator.Events;

public abstract class EventHandlerWrapper {
    public abstract Task HandleAsync(IEvent @event,
                                     IServiceProvider serviceProvider,
                                     CancellationToken cancellationToken);
}