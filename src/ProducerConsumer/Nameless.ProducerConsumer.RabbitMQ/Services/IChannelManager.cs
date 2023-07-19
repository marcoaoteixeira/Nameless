using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Services {
    public interface IChannelManager {
        #region Members

        IModel GetChannel();

        #endregion
    }
}
