namespace Nameless.Patterns.Mediator.Events;

/// <summary>
/// An interface that represents an event handler invoker.
/// </summary>
public interface IEventHandlerInvoker {
    /// <summary>
    /// Invokes the event handler based in the type of the event.
    /// </summary>
    /// <typeparam name="TEvent">Type of the event.</typeparam>
    /// <param name="evt">The event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the event handler execution.
    /// </returns>
    Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IEvent;
}