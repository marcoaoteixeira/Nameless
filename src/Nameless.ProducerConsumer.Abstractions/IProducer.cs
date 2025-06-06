namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a publisher that can publish messages to topics.
/// </summary>
public interface IProducer : IDisposable, IAsyncDisposable {
    /// <summary>
    ///     Gets the topic that this producer is associated with.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Produces a message with the provided arguments.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task ProduceAsync(object message, Args args, CancellationToken cancellationToken);
}