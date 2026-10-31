namespace Nameless.Windows.Events;

/// <summary>
///     Event Publisher Contract
/// </summary>
public interface IEventPublisher {
    /// <summary>
    ///     Registers a recipient to receive notifications from event publisher.
    /// </summary>
    /// <typeparam name="TEvent">
    ///     Type of the event.
    /// </typeparam>
    /// <param name="recipient">
    ///     The recipient.
    /// </param>
    /// <param name="handler">
    ///     The notification handler.
    /// </param>
    void Register<TEvent>(object recipient, Action<object, TEvent> handler) where TEvent : Event;

    /// <summary>
    ///     Unregisters a recipient from receive notifications of event publisher.
    /// </summary>
    /// <typeparam name="TEvent">
    ///     Type of the event.
    /// </typeparam>
    /// <param name="recipient">
    ///     The recipient.
    /// </param>
    void Unregister<TEvent>(object recipient) where TEvent : Event;

    /// <summary>
    ///     Publishes an event.
    /// </summary>
    /// <typeparam name="TEvent">
    ///     Type of the event.
    /// </typeparam>
    /// <param name="evt">
    ///     The event.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous method
    ///     execution.
    /// </returns>
    Task PublishAsync<TEvent>(TEvent evt) where TEvent : Event;
}