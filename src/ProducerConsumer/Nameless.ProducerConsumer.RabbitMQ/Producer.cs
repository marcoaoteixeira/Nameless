using Nameless.Logging;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public sealed class Producer : IProducer {
        #region Private Read-Only Fields

        private readonly IModel _channel;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get => _logger ??= NullLogger.Instance;
            set => _logger = value ?? NullLogger.Instance;
        }

        #endregion

        #region Public Constructors

        public Producer(IModel channel) {
            Prevent.Null(channel, nameof(channel));

            _channel = channel;
        }

        #endregion

        #region IProducer Members

        /// <inheritdoc />
        /// <remarks>
        /// Here, <paramref name="topic"/> will be used as the routingKey parameter when calling <see cref="IModel.BasicPublish"/>
        /// </remarks>
        public void Produce(string topic, object message, params Parameter[] parameters) {
            var producerParams = new ProducerParameters(parameters);
            var properties = _channel.CreateBasicProperties().FillWith(producerParams);
            var envelope = new Envelope(
                message: message,
                messageId: properties.MessageId,
                correlationId: properties.CorrelationId,
                publishedAt: DateTime.UtcNow
            );

            try {
                _channel.BasicPublish(
                    exchange: producerParams.ExchangeName,
                    routingKey: topic,
                    basicProperties: properties,
                    body: envelope.CreateBuffer()
                );
            } catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }
        }

        #endregion
    }
}