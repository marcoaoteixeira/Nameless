using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Services;

public interface IChannelFactory {
    #region Members

    IModel CreateChannel();

    #endregion
}