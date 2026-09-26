namespace Nameless.ProducerConsumer.RabbitMQ.ObjectModel;

/// <summary>
///     Contains identity and tracing metadata for a RabbitMQ message envelope.
/// </summary>
public record Header {
    /// <summary>
    ///     Gets the unique identifier for this message.
    /// </summary>
    public string? MessageID { get; init; }

    /// <summary>
    ///     Gets the correlation identifier used to link related messages (e.g., request/response).
    /// </summary>
    public string? CorrelationID { get; init; }

    /// <summary>
    ///     Gets the Unix timestamp (in seconds) when the message was created.
    /// </summary>
    public long Timestamp { get; init; }
}