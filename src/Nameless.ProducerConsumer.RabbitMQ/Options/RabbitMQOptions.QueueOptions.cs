namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for a RabbitMQ queue.
/// </summary>
public class QueueOptions {
    /// <summary>
    ///     Gets or sets the name of the queue.
    /// </summary>
    public string Name { get; set; } = Internals.Defaults.QUEUE_NAME;

    /// <summary>
    ///     Whether the queue is durable.
    /// </summary>
    public bool Durable { get; set; }

    /// <summary>
    ///     Whether the queue is exclusive to the connection that created it.
    /// </summary>
    public bool Exclusive { get; set; }

    /// <summary>
    ///     Whether the queue is auto-deleted when no consumers are connected.
    /// </summary>
    public bool AutoDelete { get; set; }

    /// <summary>
    ///     Gets or sets the exchange to bind the queue to.
    /// </summary>
    public string ExchangeName { get; set; } = Internals.Defaults.EXCHANGE_NAME;

    /// <summary>
    ///     Gets or sets the queue arguments.
    /// </summary>
    public Dictionary<string, object?> Arguments { get; set; } = [];

    /// <summary>
    ///     Gets or sets the bindings associated with the queue.
    /// </summary>
    public BindingOptions[] Bindings { get; set; } = [];
}