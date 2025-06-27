namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for a RabbitMQ exchange.
/// </summary>
public sealed record ExchangeSettings {
    /// <summary>
    ///     Gets or sets the name of the exchange.
    /// </summary>
    public string Name { get; set; } = Internals.Defaults.EXCHANGE_NAME;

    /// <summary>
    ///     Gets or sets the type of the exchange.
    /// </summary>
    public ExchangeType Type { get; set; }

    /// <summary>
    ///     Whether the exchange is durable.
    /// </summary>
    public bool Durable { get; set; } = true;

    /// <summary>
    ///     Whether the exchange is auto-deleted when no queues are bound to it.
    /// </summary>
    public bool AutoDelete { get; set; }

    /// <summary>
    ///     Gets or sets the arguments for the exchange.
    /// </summary>
    public Dictionary<string, object?> Arguments { get; set; } = [];
}