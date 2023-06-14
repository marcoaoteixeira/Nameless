using System.Text.Json;
using Nameless.Logging;
using Nameless.PubSub.RabbitMQ;
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

        private void OnMessage<T>(Registration<T> registration, BasicDeliverEventArgs deliverArgs, Arguments arguments) {
            Action<T>? handler;

            try { handler = registration.CreateHandler(); } catch (Exception ex) {
                Logger.Error(ex, $"({registration}) Handler creation failed. Reason: {ex.Message}");
                NoAcknowledge(_channel, deliverArgs, arguments);
                return;
            }

            if (handler == default) {
                Logger.Warning($"({registration}) No suitable handler found.");
                NoAcknowledge(_channel, deliverArgs, arguments);
                return;
            }

            var message = JsonSerializer.Deserialize<Message>(deliverArgs.Body.ToArray());
            if (message == default) {
                Logger.Warning($"({registration}) Message deserialization failed.");
                NoAcknowledge(_channel, deliverArgs, arguments);
                return;
            }

            try {
                // Here, message.Payload is a JsonElement.
                // As so, we can deserialize it to the type
                // that the handler can handle.
                if (message.Payload is JsonElement json) {
                    var payload = json.Deserialize<T>()!;
                    handler(payload);
                    Acknowledge(_channel, deliverArgs, arguments);
                } else { Logger.Information($"({registration}) Could not deserialize payload object as {typeof(T)}."); }
            } catch (Exception ex) {
                Logger.Error(ex, $"({registration}) Message could not be handled. Reason: {ex.Message}");
                NoAcknowledge(_channel, deliverArgs, arguments);
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

        private static void Acknowledge(IModel channel, BasicDeliverEventArgs deliverArgs, Arguments arguments) {
            if (!arguments.AckOnSuccess()) { return; }

            channel.BasicAck(
                deliveryTag: deliverArgs.DeliveryTag,
                multiple: arguments.AckMultiple()
            );
        }

        private static void NoAcknowledge(IModel channel, BasicDeliverEventArgs deliverArgs, Arguments arguments) {
            if (!arguments.RejectOnFailure()) { return; }

            channel.BasicNack(
                deliveryTag: deliverArgs.DeliveryTag,
                multiple: arguments.AckMultiple(),
                requeue: arguments.RequeueOnFailure()
            );
        }

        #endregion

        #region IConsumer Members

        /// <inheritdoc />
        public Registration<T> Register<T>(string topic, Action<T> callback, Arguments arguments) {
            BlockAccessAfterDispose();

            Prevent.Null(callback, nameof(callback));

            lock (_lock) {
                // create callback tag
                var tag = TagHelper.GenerateTag(callback);

                // if registration already exists, just return it.
                if (_registrations.ContainsKey(tag)) {
                    return (Registration<T>)_registrations[tag];
                }

                // creates registration
                var registration = new Registration<T>(tag, topic, callback);

                // set into cache
                _registrations.Add(registration.Tag, registration);

                // creates the consumer event
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (sender, deliverArgs)
                    => OnMessage(registration, deliverArgs, arguments);

                // attach the consumer
                var consumerTag = _channel.BasicConsume(
                    queue: topic,
                    autoAck: arguments.AutoAck(),
                    consumerTag: registration.Tag,
                    consumer: consumer
                );

                return registration;
            }
        }

        public bool Unregister<T>(Registration<T> registration) {
            BlockAccessAfterDispose();

            Prevent.Null(registration, nameof(registration));

            var tag = registration.Tag;
            if (!_registrations.ContainsKey(tag)) { return false; }

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