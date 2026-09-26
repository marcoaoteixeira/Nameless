namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a consumer.
/// </summary>
/// <typeparam name="T">
///     Type of the message this consumer can handle.
/// </typeparam>
public interface IConsumer<in T> {
    /// <summary>
    ///     Gets the consumer name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the topic associated with the consumer.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Consumes the message.
    /// </summary>
    /// <param name="value">
    ///     The message.
    /// </param>
    /// <param name="context">
    ///     The context.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous execution.
    /// </returns>
    Task ConsumeAsync(T value, ConsumerContext context, CancellationToken cancellationToken);
}