namespace Nameless.ProducerConsumer;

/// <summary>
/// Defines what a producer should do.
/// </summary>
public interface IProducer {
    /// <summary>
    /// Produces a payload for a specific topic.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload that will go inside the message.</param>
    /// <param name="args">The producer's arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ProduceAsync(string topic,
                      object payload,
                      ProducerArgs args,
                      CancellationToken cancellationToken);
}