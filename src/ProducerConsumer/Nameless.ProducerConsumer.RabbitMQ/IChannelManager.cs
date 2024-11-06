using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public interface IChannelManager {
    IModel GetChannel();
}