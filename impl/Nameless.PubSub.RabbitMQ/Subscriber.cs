using System;
using System.Collections.Generic;
using System.Linq;
using Nameless.Serialization;
using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ {

    public sealed class Subscriber : ISubscriber {
        #region Private Read-Only Fields

        private readonly IConnectionFactory _factory;
        private readonly PubSubSettings _settings;
        private readonly ISerializer _serializer;
        private readonly object _syncLock = new object ();

        #endregion

        #region Private Fields

        private Dictionary<string, ISubscriber> _cache = new Dictionary<string, ISubscriber> ();
        private bool _disposed;

        #endregion

        #region Public Constructors

        public Subscriber (IConnectionFactory factory, PubSubSettings settings, ISerializer serializer) {
            Prevent.ParameterNull (factory, nameof (factory));
            Prevent.ParameterNull (settings, nameof (settings));
            Prevent.ParameterNull (serializer, nameof (serializer));

            _factory = factory;
            _settings = settings;
            _serializer = serializer;
        }

        #endregion

        #region Destructor

        ~Subscriber () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (GetType ().Name);
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                lock (_syncLock) {
                    _cache.Values.Each (item => item.Dispose ());
                }
            }

            _cache = null;

            _disposed = true;
        }

        #endregion

        #region ISubscriber Members

        public Subscription Subscribe (string topic, Action<Message> handler) {
            BlockAccessAfterDispose ();

            Prevent.ParameterNullOrWhiteSpace (topic, nameof (topic));
            Prevent.ParameterNull (handler, nameof (handler));

            var exchange = _settings.Exchanges.FirstOrDefault (_ => _.Name == topic);
            if (exchange == null) {
                throw new InvalidOperationException ("Exchange not configured.");
            }

            lock (_syncLock) {
                if (!_cache.ContainsKey (exchange.Name)) {
                    _cache[exchange.Name] = new InnerSubscriber (_factory, exchange, _serializer);
                }
                return _cache[exchange.Name].Subscribe (exchange.Name, handler); ;
            }
        }

        public bool Unsubscribe (Subscription subscription) {
            BlockAccessAfterDispose ();

            Prevent.ParameterNull (subscription, nameof (subscription));
            lock (_syncLock) {
                var topic = subscription.Topic;
                return _cache.ContainsKey (topic) ?
                    _cache[topic].Unsubscribe (subscription) :
                    false;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion
    }
}