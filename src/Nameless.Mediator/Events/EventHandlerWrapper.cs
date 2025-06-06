namespace Nameless.Mediator.Events;

/// <summary>
///     A wrapper class for an event handler.
/// </summary>
public abstract class EventHandlerWrapper {
    /// <summary>
    ///     Handles the event asynchronously.
    /// </summary>
    /// <param name="evt">The event.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task" /> representing the handler execution.
    /// </returns>
    public abstract Task HandleAsync(IEvent evt,
                                     IServiceProvider serviceProvider,
                                     CancellationToken cancellationToken);
}