using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Services;

public interface IChannelFactory {
    IModel CreateChannel();
}