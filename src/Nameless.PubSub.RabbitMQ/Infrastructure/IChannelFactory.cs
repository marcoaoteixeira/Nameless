using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ.Infrastructure;

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
    Task<IChannel> CreateChannelAsync(string exchangeName, CancellationToken cancellationToken);
}