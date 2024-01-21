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

        #region Private Static Methods

        private static Envelope CreateEnvelope(object message, IBasicProperties properties)
            => new() {
                Message = message,
                MessageId = properties.MessageId,
                CorrelationId = properties.CorrelationId,
                PublishedAt = DateTime.UtcNow
            };

        private static IBasicProperties CreateProperties(IModel channel, ProducerArgs innerArgs)
            => channel
                .CreateBasicProperties()
                .FillWith(innerArgs);

        #endregion

        #region IProducerService Members

        public Task ProduceAsync(string topic, object message, ProducerArgs? args, CancellationToken cancellationToken) {
            var innerArgs = args ?? ProducerArgs.Empty;
            var properties = CreateProperties(_channel, innerArgs);
            var envelope = CreateEnvelope(message, properties);

            try {
                var routingKeys = innerArgs.GetRoutingKeys().Append(topic);
                var batch = _channel.CreateBasicPublishBatch();
                var buffer = envelope.CreateBuffer();

                foreach (var routingKey in routingKeys) {
                    batch.Add(
                        exchange: innerArgs.GetExchangeName(),
                        routingKey: topic,
                        mandatory: innerArgs.GetMandatory(),
                        properties: properties,
                        body: buffer
                    );
                }

                batch.Publish();
            } catch (Exception ex) { _logger.LogError(ex, "{Message}", ex.Message); throw; }

            return Task.CompletedTask;
        }

        #endregion
    }
}
