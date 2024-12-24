using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ;

/// <summary>
/// Chanel provider contract.
/// </summary>
public interface IChannelFactory {
    /// <summary>
    /// Creates a new RabbitMQ <see cref="IChannel"/>.
    /// </summary>
    /// <param name="exchangeName">The exchange name which will be associated to this channel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A new <see cref="IChannel"/> object.</returns>
    Task<IChannel> CreateChannelAsync(string exchangeName, CancellationToken cancellationToken);
}