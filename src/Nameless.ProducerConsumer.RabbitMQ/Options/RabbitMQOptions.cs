namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for RabbitMQ.
/// </summary>
public sealed class RabbitMQOptions {
    public TimeSpan ConsumerStartupTimeout { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    ///     Gets or sets the server settings for RabbitMQ.
    /// </summary>
    public ServerSettings Server { get; set; } = new();

    /// <summary>
    ///     Gets or sets the exchanges to be used in RabbitMQ.
    /// </summary>
    public ExchangeSettings[] Exchanges = [];

    /// <summary>
    ///     Gets or sets the queues to be used in RabbitMQ.
    /// </summary>
    public QueueSettings[] Queues { get; set; } = [];

    /// <summary>
    ///     Gets or sets the prefetch settings for RabbitMQ consumers.
    /// </summary>
    public PrefetchSettings Prefetch { get; set; } = new();
}