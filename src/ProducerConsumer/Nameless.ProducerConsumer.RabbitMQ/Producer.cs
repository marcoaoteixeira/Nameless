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

        #region IPublisher Members

        /// <inheritdoc />
        /// <remarks>
        /// Here, topic will be used as the routingKey parameter when calling <see cref="IModel.BasicPublish"/>
        /// </remarks>
        public Task<string> ProduceAsync(string topic, object payload, Arguments arguments, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();

            var properties = _channel.CreateBasicProperties().FillWith(arguments);
            var message = new Message(
                id: properties.MessageId,
                correlationId: properties.CorrelationId,
                payload: payload,
                publishedAt: DateTime.UtcNow
            );

            try {
                _channel.BasicPublish(
                    exchange: arguments.ExchangeName(),
                    routingKey: topic,
                    basicProperties: properties,
                    body: message.AsBuffer()
                );
            } catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }

            return Task.FromResult(message.Id);
        }

        #endregion
    }
}