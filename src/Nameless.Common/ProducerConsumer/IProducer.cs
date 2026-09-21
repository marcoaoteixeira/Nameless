namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a producer.
/// </summary>
public interface IProducer {
    /// <summary>
    ///     Produces a message to the specified topic.
    /// </summary>
    /// <param name="topic">
    ///     The topic.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="context">
    ///     The producer context.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous execution.
    /// </returns>
    Task ProduceAsync<T>(string topic, T value, ProducerContext context, CancellationToken cancellationToken);
}