using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Services {
    public interface IConnectionManager {
        #region Methods

        IConnection GetConnection();

        #endregion
    }
}
