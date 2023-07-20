using Microsoft.Extensions.Hosting;

namespace Nameless.ProducerConsumer.AspNetCore {
    public abstract class ConsumerBackgroundService<T> : BackgroundService {
        #region Private Read-Only Fields

        private readonly IConsumerService _consumerService;

        #endregion

        #region Private Fields

        private IDisposable? _registration;
        private bool _disposed;

        #endregion

        #region Public Abstract Properties

        public abstract string Topic { get; }
        public abstract ConsumerArgs? Args { get; }

        #endregion

        #region Protected Constructors

        protected ConsumerBackgroundService(IConsumerService consumer) {
            Prevent.Against.Null(consumer, nameof(consumer));

            _consumerService = consumer;
        }

        #endregion

        #region Public Override Methods

        public override Task StartAsync(CancellationToken cancellationToken) {
            InitializeRegistration();

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            TerminateRegistration();

            return base.StopAsync(cancellationToken);
        }

        public override void Dispose() {
            if (_disposed) { return; }

            TerminateRegistration();

            _disposed = true;

            base.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract Task Consume(T message, CancellationToken cancellationToken = default);

        #endregion

        #region Private Methods

        private void InitializeRegistration() {
            _registration ??= _consumerService.Register<T>(Topic, Consume, Args);
        }

        private void TerminateRegistration() {
            if (_registration != null) {
                _consumerService.Unregister((Registration<T>)_registration);
                _registration = null;
            }
        }

        #endregion
    }
}
