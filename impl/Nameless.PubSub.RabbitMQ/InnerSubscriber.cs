using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nameless.Logging;
using Nameless.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.PubSub.RabbitMQ {
    internal sealed class InnerSubscriber : ISubscriber {

        #region Private Read-Only Fields

        private readonly Exchange _exchange;
        private readonly ISerializer _serializer;
        private readonly object _syncLock = new object ();

        #endregion

        #region Private Fields

        private ISet<Subscription> _cache = new HashSet<Subscription> ();
        private IConnection _connection;
        private IModel _channel;
        private bool _disposed;

        #endregion

        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Internal Constructors

        internal InnerSubscriber (IConnectionFactory factory, Exchange exchange, ISerializer serializer) {
            Prevent.ParameterNull (factory, nameof (factory));
            Prevent.ParameterNull (exchange, nameof (exchange));
            Prevent.ParameterNull (serializer, nameof (serializer));

            _connection = factory.CreateConnection ();
            _channel = _connection.CreateModel ();
            _exchange = exchange;
            _serializer = serializer;

            Initialize ();
        }

        #endregion

        #region Destructor

        ~InnerSubscriber () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            _channel.ExchangeDeclare (
                exchange: _exchange.Name,
                type: _exchange.Type.GetDescription (),
                durable: _exchange.Durable,
                autoDelete: _exchange.AutoDelete,
                arguments: null
            );

            var queueName = _channel.QueueDeclare ().QueueName;
            _channel.QueueBind (
                queue: queueName,
                exchange: _exchange.Name,
                routingKey: _exchange.RoutingKey,
                arguments: null
            );

            var consumer = new EventingBasicConsumer (_channel);
            consumer.Received += (model, args) => { OnMessage (args); };

            _channel.BasicConsume (
                queue: queueName,
                autoAck: _exchange.AutoAck,
                consumer: consumer
            );
        }

        // TODO : REFACTOR
        private void OnMessage (BasicDeliverEventArgs args) {
            var payload = args.Body;
            var message = _serializer.Deserialize<Message> (payload);

            lock (_syncLock) {
                Logger.Debug ("Starting send messages...");
                foreach (var subscription in _cache) {
                    Action<Message> handler = null;
                    try { handler = subscription.CreateHandler (); } catch (ObjectDisposedException) { /* Just ignore */ } catch (Exception ex) { Logger.Error (ex, ex.Message); throw; }
                    if (handler == null) { continue; }
                    Logger.Debug ("Sending message to handler attached to topic {0}...", subscription.Topic);
                    Task.Run (() => handler (message)).ContinueWith (continuation => {

                        if (!_exchange.AutoAck) {
                            _channel.BasicAck (
                                deliveryTag: args.DeliveryTag,
                                multiple: _exchange.AckMultiple
                            );
                        }

                        var exception = continuation.Exception;
                        if (exception != null) {
                            var flatten = exception.Flatten ();
                            Logger.Error (flatten, flatten.Message);
                        }
                    });
                    Logger.Debug ("Done.");
                }
                Logger.Debug ("Done sending messages.");
            }
        }

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (GetType ().FullName);
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                lock (_syncLock) {
                    _cache.Each (item => ((IDisposable)item).Dispose ());
                    _cache.Clear ();
                }

                if (_channel != null) { _channel.Dispose (); }
                if (_connection != null) { _connection.Dispose (); }
            }

            _cache = null;
            _channel = null;
            _connection = null;

            _disposed = true;
        }

        #endregion

        #region ISubscriber Members

        Subscription ISubscriber.Subscribe (string topic, Action<Message> handler) {
            BlockAccessAfterDispose ();

            Prevent.ParameterNullOrWhiteSpace (topic, nameof (topic));
            Prevent.ParameterNull (handler, nameof (handler));

            lock (_syncLock) {
                var subscription = new Subscription (topic, handler);
                _cache.Add (subscription);
                return subscription;
            }
        }

        bool ISubscriber.Unsubscribe (Subscription subscription) {
            BlockAccessAfterDispose ();

            Prevent.ParameterNull (subscription, nameof (subscription));

            lock (_syncLock) {
                var result = _cache.Remove (subscription);
                if (result) { subscription.Dispose (); }
                return result;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (obj: this);
        }

        #endregion
    }
}