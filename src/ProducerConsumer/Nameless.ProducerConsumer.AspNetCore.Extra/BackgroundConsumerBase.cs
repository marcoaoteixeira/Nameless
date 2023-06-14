using Microsoft.Extensions.Hosting;

namespace Nameless.ProducerConsumer.AspNetCore {
    public abstract class BackgroundConsumerBase<T> : BackgroundService {
        #region Private Read-Only Fields

        private readonly IConsumer _consumer;
        private readonly string _topic;
        private readonly Arguments _arguments;

        #endregion

        #region Private Fields

        private IDisposable? _registration;
        private bool _disposed;

        #endregion

        #region Protected Constructors

        protected BackgroundConsumerBase(IConsumer consumer, string topic, Arguments? arguments = default) {
            Prevent.Null(consumer, nameof(consumer));

            _consumer = consumer;
            _topic = topic;
            _arguments = arguments ?? Arguments.Empty;
        }

        #endregion

        #region Public Override Methods

        public override void Dispose() {
            base.Dispose();

            if (_disposed) { return; }
            if (_registration != null) {
                _registration.Dispose();
                _registration = null;
            }
            _disposed = true;
        }

        #endregion

        #region Protected Override Methods

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            _registration = _consumer.Register<T>(_topic, Consume, _arguments);

            return Task.CompletedTask;
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract void Consume(T payload);

        #endregion
    }
}
