namespace Nameless.Mediator.Events;

/// <summary>
///     Defines an event handler invoker.
/// </summary>
public interface IEventHandlerInvoker {
    /// <summary>
    ///     Invokes the event handler based in the type of the event.
    /// </summary>
    /// <typeparam name="TEvent">
    ///     Type of the event.
    /// </typeparam>
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
    /// <remarks>
    ///     All handlers run concurrently, so handlers must not share state
    ///     that isn't thread-safe (e.g. a scoped <c>DbContext</c>). A failing
    ///     handler does not stop the others: a single failure is rethrown
    ///     as-is, several failures are thrown as an
    ///     <see cref="AggregateException"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="evt"/> is <see langword="null"/>.
    /// </exception>
    Task PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IEvent;
}