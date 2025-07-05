using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
///     Represents a RabbitMQ channel configurator.
/// </summary>
public interface IChannelConfigurator {
    /// <summary>
    ///     Configures the specified RabbitMQ channel.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="queueName">The queue name.</param>
    /// <param name="usePrefetch">Whether it should use the prefetch settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task ConfigureAsync(IChannel channel, string queueName, bool usePrefetch, CancellationToken cancellationToken);
}