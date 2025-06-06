namespace Nameless.PubSub;

/// <summary>
///     Subscriber Contract.
/// </summary>
public interface ISubscriber {
    /// <summary>
    ///     Subscribes to a topic.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="messageHandler">The message handler.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The subscription identification.</returns>
    Task<string> SubscribeAsync(string topic,
                                MessageHandlerDelegate messageHandler,
                                SubscriberArgs args,
                                CancellationToken cancellationToken);

    /// <summary>
    ///     Unsubscribes a subscriber.
    /// </summary>
    /// <param name="subscription">The subscription.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if it was unsubscribed; otherwise, <c>false</c>.</returns>
    Task<bool> UnsubscribeAsync(string subscription,
                                CancellationToken cancellationToken);
}