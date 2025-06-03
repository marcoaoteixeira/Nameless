using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
/// Represents a factory for creating RabbitMQ channels.
/// </summary>
public interface IChannelFactory {
    /// <summary>
    ///     Creates a new RabbitMQ <see cref="IChannel" />.
    /// </summary>
    /// <param name="exchangeName">The exchange which will be associated with the channel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A new <see cref="IChannel" /> instance.</returns>
    Task<IChannel> CreateAsync(string exchangeName, CancellationToken cancellationToken);
}