namespace Nameless.Mediator.Events;

/// <summary>
///     Defines an event handler.
/// </summary>
/// <typeparam name="TEvent">
///     Type of the event
/// </typeparam>
public interface IEventHandler<in TEvent>
    where TEvent : IEvent {
    /// <summary>
    ///     Handles the event asynchronously.
    /// </summary>
    /// <param name="evt">
    ///     The event.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing the action
    ///     asynchronous operation.
    /// </returns>
    Task HandleAsync(TEvent evt, CancellationToken cancellationToken);
}