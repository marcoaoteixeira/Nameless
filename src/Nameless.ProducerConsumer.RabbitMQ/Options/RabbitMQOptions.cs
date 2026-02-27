using Nameless.Attributes;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Represents the settings for RabbitMQ.
/// </summary>
[ConfigurationSectionName("RabbitMQ")]
public class RabbitMQOptions {
    public TimeSpan ConsumerStartupTimeout { get; set; } = TimeSpan.FromSeconds(seconds: 1);

    /// <summary>
    ///     Gets or sets the server settings for RabbitMQ.
    /// </summary>
    public ServerOptions Server { get; set; } = new();

    /// <summary>
    ///     Gets or sets the exchanges to be used in RabbitMQ.
    /// </summary>
    public ExchangeOptions[] Exchanges = [];

    /// <summary>
    ///     Gets or sets the queues to be used in RabbitMQ.
    /// </summary>
    public QueueOptions[] Queues { get; set; } = [];

    /// <summary>
    ///     Gets or sets the prefetch settings for RabbitMQ consumers.
    /// </summary>
    public PrefetchOptions Prefetch { get; set; } = new();
}