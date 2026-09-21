namespace Nameless.ProducerConsumer.RabbitMQ.ObjectModel;

/// <summary>
///     Represents the outer wrapper for a RabbitMQ message, combining routing/tracing
///     metadata with the actual message payload.
/// </summary>
public record Message<T> {
    /// <summary>
    ///     Gets the envelope header containing message identity and tracing metadata.
    /// </summary>
    public required Header Header { get; init; }

    /// <summary>
    ///     Gets the message payload.
    /// </summary>
    public required T Content { get; init; }
}