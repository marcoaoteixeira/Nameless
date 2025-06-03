namespace Nameless.PubSub;

/// <summary>
///     Represents a publisher that can publish messages to topics.
/// </summary>
public interface IPublisher {
    /// <summary>
    ///     Publishes a message to a topic.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="message">The message.</param>
    /// <param name="args">The publisher arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that will complete when the message gets published.</returns>
    Task PublishAsync(string topic,
                      object message,
                      PublisherArgs args,
                      CancellationToken cancellationToken);
}