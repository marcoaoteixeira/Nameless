using System.Text;
using System.Text.Json;
using Nameless.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public sealed class Consumer : IConsumer, IDisposable {
        #region Private Read-Only Fields

        private readonly IModel _channel;
        private readonly object _lock = new();

        #endregion

        #region Private Fields

        private Dictionary<string, IDisposable> _registrations = new();
        private bool _disposed;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get => _logger ??= NullLogger.Instance;
            set => _logger = value ?? NullLogger.Instance;
        }

        #endregion

        #region Public Constructors

        public Consumer(IModel channel) {
            Prevent.Null(channel, nameof(channel));

            _channel = channel;
        }

        #endregion

        #region Destructor

        ~Consumer() => Dispose(disposing: false);

        #endregion

        #region Private Methods

        private async Task OnMessage<T>(Registration<T> registration, BasicDeliverEventArgs deliverArgs, ConsumerParameters parameters) {
            MessageEventHandler<T>? handler;

            try { handler = registration.CreateHandler(); }
            catch (Exception ex) {
                if (ex is ObjectDisposedException) {
                    Unregister(registration);
                }

                Logger.Error(ex, $"{registration}: Handler creation failed. Reason: {ex.Message}");
                NAck(_channel, deliverArgs, parameters);
                
                return;
            }

            if (handler == null) {
                Logger.Warning($"{registration}: No suitable handler found.");
                NAck(_channel, deliverArgs, parameters);

                return;
            }

            var envelope = JsonSerializer.Deserialize<Envelope>(deliverArgs.Body.ToArray());
            if (envelope == null) {
                Logger.Warning($"{registration}: Envelope deserialization failed.");
                NAck(_channel, deliverArgs, parameters);

                return;
            }

            // Here, envelope.Message is a JsonElement.
            // So, we'll deserialize it to the type that the handler is expecting.
            if (envelope.Message is not JsonElement json) {
                Logger.Warning($"{registration}: Message is not a {typeof(JsonElement)}.");
                NAck(_channel, deliverArgs, parameters);

                return;
            }

            if (json.Deserialize<T>() is not T message) {
                Logger.Warning($"{registration}: Unable to deserialize the message to expecting type {typeof(T)}.");
                NAck(_channel, deliverArgs, parameters);

                return;
            }

            try {
                await handler(message);
                Ack(_channel, deliverArgs, parameters);

            } catch (Exception ex) {
                Logger.Error(ex, $"{registration}: Error when handling the message. Reason: {ex.Message}");
                NAck( _channel, deliverArgs, parameters);
            }
        }

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                lock (_lock) {
                    _registrations.Values.Each(_ => _.Dispose());
                    _registrations.Clear();
                }
            }

            _registrations = default!;

            _disposed = true;
        }

        #endregion

        #region Private Static Methods

        private static string GenerateTag<T>(MessageEventHandler<T> handler) {
            if (handler == null) { return string.Empty; }

            var method = handler.Method;
            var parameters = method.GetParameters().Select(_ => $"{_.ParameterType.Name} {_.Name}").ToArray();
            var signature = $"{method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", parameters)})";
            var buffer = Encoding.UTF8.GetBytes(signature);

            return Convert.ToBase64String(buffer);
        }

        private static void Ack(IModel channel, BasicDeliverEventArgs deliverArgs, ConsumerParameters parameters) {
            if (!parameters.AckOnSuccess) { return; }

            channel.BasicAck(
                deliveryTag: deliverArgs.DeliveryTag,
                multiple: parameters.AckMultiple
            );
        }

        private static void NAck(IModel channel, BasicDeliverEventArgs deliverArgs, ConsumerParameters parameters) {
            if (!parameters.NAckOnFailure) { return; }

            channel.BasicNack(
                deliveryTag: deliverArgs.DeliveryTag,
                multiple: parameters.NAckMultiple,
                requeue: parameters.RequeueOnFailure
            );
        }

        #endregion

        #region IConsumer Members

        /// <inheritdoc />
        public Registration<T> Register<T>(string topic, MessageEventHandler<T> handler, params Parameter[] parameters) {
            BlockAccessAfterDispose();

            Prevent.Null(handler, nameof(handler));

            lock (_lock) {
                var consumerParams = new ConsumerParameters(parameters);

                // create callback tag
                var tag = GenerateTag(handler);

                // if registration already exists, just return it.
                if (_registrations.ContainsKey(tag)) {
                    return (Registration<T>)_registrations[tag];
                }

                // creates registration
                var registration = new Registration<T>(tag, topic, handler);

                // set into cache
                _registrations.Add(tag, registration);

                // creates the consumer event
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += (sender, deliverArgs)
                    => OnMessage(registration, deliverArgs, consumerParams);

                // attach the consumer
                var consumerTag = _channel.BasicConsume(
                    queue: topic,
                    autoAck: consumerParams.AutoAck,
                    consumerTag: tag,
                    consumer: consumer
                );

                return registration;
            }
        }

        public bool Unregister<T>(Registration<T> registration) {
            BlockAccessAfterDispose();

            Prevent.Null(registration, nameof(registration));

            var tag = registration.Tag;

            if (!_registrations.ContainsKey(tag)) {
                return false;
            }

            lock (_lock) {
                _channel.BasicCancel(tag);
                _registrations[tag].Dispose();
                _registrations.Remove(tag);
                return true;
            }
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