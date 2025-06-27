namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a producer.
/// </summary>
public interface IProducer : IDisposable, IAsyncDisposable {
    /// <summary>
    ///     Gets the topic that this producer is associated with.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Produces a message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task ProduceAsync(object message, Parameters parameters, CancellationToken cancellationToken);
}
