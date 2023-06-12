using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {

    public interface IChannelFactory {

        #region Methods

        IModel Create(IEnumerable<ExchangeSettings> exchanges);

        #endregion
    }
}
