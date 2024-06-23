using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public sealed class ProducerService : IProducerService {
        #region Private Read-Only Fields

        private readonly IModel _channel;
        private readonly ILogger _logger;

        #endregion

        #region Public Constructors

        public ProducerService(IModel channel, ILogger<ProducerService> logger) {
            _channel = Guard.Against.Null(channel, nameof(channel));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Private Static Methods

        private static Envelope CreateEnvelope(object message, IBasicProperties properties)
            => new(Message: message,
                   MessageId: properties.MessageId,
                   CorrelationId: properties.CorrelationId,
                   PublishedAt: DateTime.UtcNow);

        private static IBasicProperties CreateProperties(IModel channel, ProducerArgs innerArgs)
            => channel
                .CreateBasicProperties()
                .FillWith(innerArgs);

        #endregion

        #region IProducerService Members

        public Task ProduceAsync(string topic, object message, ProducerArgs? args = null, CancellationToken cancellationToken = default) {
            var innerArgs = args ?? ProducerArgs.Empty;
            var properties = CreateProperties(_channel, innerArgs);
            var envelope = CreateEnvelope(message, properties);

            try {
                var routingKeys = innerArgs.GetRoutingKeys().Append(topic);
                var batch = _channel.CreateBasicPublishBatch();
                var buffer = envelope.CreateBuffer();

                foreach (var routingKey in routingKeys) {
                    batch.Add(exchange: innerArgs.GetExchangeName(),
                              routingKey: routingKey,
                              mandatory: innerArgs.GetMandatory(),
                              properties: properties,
                              body: buffer);
                }

                batch.Publish();
            } catch (Exception ex) { _logger.LogError(ex, "Error while publishing message."); throw; }

            return Task.CompletedTask;
        }

        #endregion
    }
}
