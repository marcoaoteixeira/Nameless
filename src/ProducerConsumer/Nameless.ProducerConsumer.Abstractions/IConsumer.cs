namespace Nameless.ProducerConsumer;

/// <summary>
/// Defines what a consumer should do.
/// </summary>
public interface IConsumer {
    /// <summary>
    /// Registers a message handler that handles messages for a specific topic.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="messageHandler">The message handler.</param>
    /// <param name="args">Consumer arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns a <see cref="Task{TResult}"/>, where <c>TResult</c> is <see cref="Registration{TMessage}"/>, representing the consumer registration action.</returns>
    Task<Registration<TMessage>> RegisterAsync<TMessage>(string topic,
                                                         MessageHandlerAsync<TMessage> messageHandler,
                                                         ConsumerArgs args,
                                                         CancellationToken cancellationToken);

    /// <summary>
    /// Unregisters a consumer.
    /// </summary>
    /// <param name="registration">The registration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if unregistered; otherwise, <c>false</c>.</returns>
    Task<bool> UnregisterAsync<TMessage>(Registration<TMessage> registration,
                                         CancellationToken cancellationToken);
}