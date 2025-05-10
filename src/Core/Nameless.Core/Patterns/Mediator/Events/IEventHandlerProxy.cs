namespace Nameless.Patterns.Mediator.Events;

/// <summary>
/// The base interface for an event handler proxy.
/// </summary>
public interface IEventHandlerProxy {
    Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IEvent;
}