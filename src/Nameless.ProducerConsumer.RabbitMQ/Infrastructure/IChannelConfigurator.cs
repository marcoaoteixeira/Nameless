using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
/// Configures a RabbitMQ channel with the specified exchange name.
/// </summary>
public interface IChannelConfigurator {
    /// <summary>
    /// Configures the specified RabbitMQ channel with the given exchange name.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="exchangeName">The exchange name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task ConfigureChannelAsync(IChannel channel, string exchangeName, CancellationToken cancellationToken);
}