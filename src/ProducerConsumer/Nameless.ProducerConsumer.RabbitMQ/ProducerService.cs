using Nameless.Logging;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public sealed class ProducerService : IProducerService {
        #region Private Read-Only Fields

        private readonly IModel _channel;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Public Constructors

        public ProducerService(IModel channel) {
            _channel = Prevent.Against.Null(channel, nameof(channel));
        }

        #endregion

        #region IProducerService Members

        public Task ProduceAsync(string topic, object message, ProducerArgs? args = null, CancellationToken cancellationToken = default) {
            var innerArgs = args ?? ProducerArgs.Empty;
            var properties = _channel.CreateBasicProperties().FillWith(innerArgs);
            var envelope = new Envelope(
                message: message,
                messageId: properties.MessageId,
                correlationId: properties.CorrelationId,
                publishedAt: DateTime.UtcNow
            );

            try {
                _channel.BasicPublish(
                    exchange: innerArgs.GetExchangeName(),
                    routingKey: topic,
                    basicProperties: properties,
                    body: envelope.CreateBuffer()
                );
            } catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }

            return Task.CompletedTask;
        }

        #endregion
    }
}
