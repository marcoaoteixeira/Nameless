using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public sealed class ConsumerService : IConsumerService, IDisposable {
        #region Private Read-Only Fields

        private readonly IModel _channel;

        #endregion

        #region Private Fields

        private ConcurrentDictionary<string, IDisposable> _registrations = new();
        private bool _disposed;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Public Constructors

        public ConsumerService(IModel channel) {
            _channel = Prevent.Against.Null(channel, nameof(channel));
        }

        #endregion

        #region Destructor

        ~ConsumerService() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(ConsumerService));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                var registrations = _registrations.Values.ToArray();
                _registrations.Clear();

                foreach (var registration in registrations) {
                    registration.Dispose();
                }
            }

            _registrations = null!;
            _disposed = true;
        }

        private async Task OnMessage<T>(Registration<T> registration, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
            MessageHandler<T>? handler;

            try { handler = registration.CreateHandler(); } catch (Exception ex) {
                if (ex is ObjectDisposedException) {
                    Unregister(registration);
                }

                Logger.LogError(ex, $"{registration}: Handler creation failed. Reason: {ex.Message}");
                NAck(_channel, deliverEventArgs, consumerArgs);

                return;
            }

            if (handler == null) {
                Logger.LogWarning($"{registration}: No suitable handler found.");
                NAck(_channel, deliverEventArgs, consumerArgs);

                return;
            }

            var envelope = JsonSerializer.Deserialize<Envelope>(deliverEventArgs.Body.ToArray());
            if (envelope == null) {
                Logger.LogWarning($"{registration}: Envelope deserialization failed.");
                NAck(_channel, deliverEventArgs, consumerArgs);

                return;
            }

            // Here, envelope.Message is a JsonElement.
            // So, we'll deserialize it to the type that the handler is expecting.
            if (envelope.Message is not JsonElement json) {
                Logger.LogWarning($"{registration}: Message is not a {typeof(JsonElement)}.");
                NAck(_channel, deliverEventArgs, consumerArgs);

                return;
            }

            if (json.Deserialize<T>() is not T message) {
                Logger.LogWarning($"{registration}: Unable to deserialize the message to expecting type {typeof(T)}.");
                NAck(_channel, deliverEventArgs, consumerArgs);

                return;
            }

            try {
                await handler(message);
                Ack(_channel, deliverEventArgs, consumerArgs);
            } catch (Exception ex) {
                Logger.LogError(ex, $"{registration}: Error when handling the message. Reason: {ex.Message}");
                NAck(_channel, deliverEventArgs, consumerArgs);
            }
        }

        #endregion

        #region Private Static Methods

        private static string GenerateTag<T>(MessageHandler<T> handler) {
            if (handler == null) { return string.Empty; }

            var method = handler.Method;
            var parameters = method.GetParameters().Select(_ => $"{_.ParameterType.Name} {_.Name}").ToArray();
            var signature = $"{method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", parameters)})";
            var buffer = Encoding.UTF8.GetBytes(signature);

            return Convert.ToBase64String(buffer);
        }

        private static void Ack(IModel channel, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
            if (!consumerArgs.GetAckOnSuccess()) { return; }

            channel.BasicAck(
                deliveryTag: deliverEventArgs.DeliveryTag,
                multiple: consumerArgs.GetAckMultiple()
            );
        }

        private static void NAck(IModel channel, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
            if (!consumerArgs.GetNAckOnFailure()) { return; }

            channel.BasicNack(
                deliveryTag: deliverEventArgs.DeliveryTag,
                multiple: consumerArgs.GetNAckMultiple(),
                requeue: consumerArgs.GetRequeueOnFailure()
            );
        }

        #endregion

        #region IConsumerService Members

        public Registration<T> Register<T>(string topic, MessageHandler<T> handler, ConsumerArgs? args = null) {
            BlockAccessAfterDispose();

            var consumerArgs = args ?? ConsumerArgs.Empty;

            // create callback tag
            var tag = GenerateTag(handler);

            var registration = _registrations.GetOrAdd(tag, tag => {
                Logger.LogInformation($"Initialize registration of consumer: {tag}");
                Logger.LogInformation($"Consumer arguments: {args.ToJson()}");

                // creates registration
                var registration = new Registration<T>(tag, topic, handler);

                // creates the consumer event
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += (sender, deliverEventArgs)
                    => OnMessage(registration, deliverEventArgs, consumerArgs);

                // attach the consumer
                var consumerTag = _channel.BasicConsume(
                    queue: topic,
                    autoAck: consumerArgs.GetAutoAck(),
                    consumerTag: tag,
                    consumer: consumer
                );

                return registration;
            });

            return (Registration<T>)registration;
        }

        public bool Unregister<T>(Registration<T> registration) {
            BlockAccessAfterDispose();

            Prevent.Against.Null(registration, nameof(registration));

            if (_registrations.Remove(registration.Tag, out var disposable)) {
                _channel.BasicCancel(registration.Tag);
                disposable.Dispose();

                return true;
            }

            return false;
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        #endregion
    }
}
