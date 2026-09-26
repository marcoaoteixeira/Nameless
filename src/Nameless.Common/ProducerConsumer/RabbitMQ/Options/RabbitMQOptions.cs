using Nameless.Configuration;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Rabbit MQ Options.
/// </summary>
[ConfigurationSectionName("RabbitMQ")]
public record RabbitMQOptions {
    /// <summary>
    ///     Gets or sets the server options.
    /// </summary>
    public ServerOptions Server { get; init; } = new();

    /// <summary>
    ///     Gets or sets the queues options.
    /// </summary>
    
    public QueueOptions[] Queues { get; init; } = [];

    /// <summary>
    ///     Gets or sets the consumer options.
    /// </summary>

    public ConsumerOptions[] Consumers { get; init; } = [];
}
