using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public sealed class ProducerService : IProducerService {
        #region Private Read-Only Fields

        private readonly IModel _channel;
        private readonly ILogger _logger;

        #endregion

        #region Public Constructors

        public ProducerService(IModel channel)
            : this(channel, NullLogger.Instance) { }

        public ProducerService(IModel channel, ILogger logger) {
            _channel = Guard.Against.Null(channel, nameof(channel));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region IProducerService Members

        public Task ProduceAsync(string topic, object message, ProducerArgs? args = null, CancellationToken cancellationToken = default) {
            var innerArgs = args ?? ProducerArgs.Empty;
            var properties = _channel.CreateBasicProperties().FillWith(innerArgs);
            var envelope = new Envelope {
                Message = message,
                MessageId = properties.MessageId,
                CorrelationId = properties.CorrelationId,
                PublishedAt = DateTime.UtcNow
            };

            try {
                _channel.BasicPublish(
                    exchange: innerArgs.GetExchangeName(),
                    routingKey: topic,
                    basicProperties: properties,
                    body: envelope.CreateBuffer()
                );
            } catch (Exception ex) {
                _logger.LogError(ex, "{Message}", ex.Message);
                throw;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
