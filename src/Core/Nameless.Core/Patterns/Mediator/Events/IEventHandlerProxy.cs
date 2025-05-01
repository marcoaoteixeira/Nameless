namespace Nameless.Patterns.Mediator.Events;

public interface IEventHandlerProxy {
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IEvent;
}