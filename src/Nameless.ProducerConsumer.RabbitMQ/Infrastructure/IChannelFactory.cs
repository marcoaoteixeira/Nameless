using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
/// Represents a factory for creating RabbitMQ channels.
/// </summary>
public interface IChannelFactory {
    /// <summary>
    ///     Creates a new RabbitMQ <see cref="IChannel" />.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     An asynchronous task representing the execution where
    ///     <see cref="Task{TResult}.Result"/> is the <see cref="IChannel" />
    ///     instance.
    /// </returns>
    Task<IChannel> CreateAsync(CancellationToken cancellationToken);
}