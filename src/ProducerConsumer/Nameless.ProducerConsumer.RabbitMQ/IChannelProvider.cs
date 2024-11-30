using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public interface IChannelProvider {
    Task<IChannel> CreateChannelAsync(string exchangeName, CancellationToken cancellationToken);
}