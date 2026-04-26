using Nameless.Attributes;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for a RabbitMQ queue.
/// </summary>
[ConfigurationSectionName("Queues")]
public record QueueOptions {
    /// <summary>
    ///     Whether the queue is durable.
    /// </summary>
    public bool Durable { get; init; }

    /// <summary>
    ///     Whether the queue is exclusive to the connection that created it.
    /// </summary>
    public bool Exclusive { get; init; }

    /// <summary>
    ///     Whether the queue is auto-deleted when no consumers are connected.
    /// </summary>
    public bool AutoDelete { get; init; }

    /// <summary>
    ///     Gets or sets the exchange to bind the queue to.
    /// </summary>
    public string ExchangeName { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the queue arguments.
    /// </summary>
    public Dictionary<string, object?> Arguments { get; init; } = [];

    /// <summary>
    ///     Gets or sets the bindings associated with the queue.
    /// </summary>
    public BindingOptions[] Bindings { get; init; } = [];
}