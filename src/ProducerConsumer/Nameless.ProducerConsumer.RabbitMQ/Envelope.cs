namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
/// Represents the envelope that will hold the data when
/// sending a message for the RabbitMQ broker.
/// </summary>
public sealed record Envelope {
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public object? Message { get; init; }
    /// <summary>
    /// Gets or sets the message id.
    /// </summary>
    public string? MessageId { get; init; }
    /// <summary>
    /// Gets or sets the message correlation id.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets or sets the message published date.
    /// </summary>
    public DateTimeOffset PublishedAt { get; init; }
}