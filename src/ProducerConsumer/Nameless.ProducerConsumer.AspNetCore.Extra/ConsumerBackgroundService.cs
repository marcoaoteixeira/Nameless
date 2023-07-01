using Microsoft.Extensions.Hosting;

namespace Nameless.ProducerConsumer.AspNetCore {
    public abstract class ConsumerBackgroundService<T> : BackgroundService {
        #region Private Read-Only Fields

        private readonly IConsumer _consumer;

        #endregion

        #region Private Fields

        private IDisposable? _registration;
        private bool _disposed;

        #endregion

        #region Public Abstract Properties

        public abstract string Topic { get; }
        public Parameter[] Parameters { get; } = Array.Empty<Parameter>();

        #endregion

        #region Protected Constructors

        protected ConsumerBackgroundService(IConsumer consumer) {
            Garda.Prevent.Null(consumer, nameof(consumer));

            _consumer = consumer;
        }

        #endregion

        #region Public Override Methods

        public override Task StartAsync(CancellationToken cancellationToken) {
            _registration ??= _consumer.Register<T>(Topic, Consume, Parameters);

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            if (_registration != null) {
                _consumer.Unregister((Registration<T>)_registration);
            }

            return base.StopAsync(cancellationToken);
        }

        public override void Dispose() {
            if (_disposed) { return; }
            if (_registration != null) {
                _registration.Dispose();
                _registration = null;
            }
            _disposed = true;

            base.Dispose();
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract Task Consume(T message, CancellationToken cancellationToken = default);

        #endregion
    }
}
