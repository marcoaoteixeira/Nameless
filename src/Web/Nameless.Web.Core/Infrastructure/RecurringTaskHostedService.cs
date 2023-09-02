using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Web.Infrastructure {
    public abstract class RecurringTaskHostedService : IHostedService {
        #region Private Read-Only Fields

        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private PeriodicTimer? _timer;

        #endregion

        #region Public Properties

        public TimeSpan Interval { get; }
        public bool Enabled { get; set; }

        #endregion

        #region Protected Constructors

        protected RecurringTaskHostedService(TimeSpan interval, bool enabled = false, ILogger? logger = null) {
            if (interval <= TimeSpan.Zero) {
                throw new ArgumentOutOfRangeException(nameof(interval), "Parameter must be positive non-zero value.");
            }

            Interval = interval;
            Enabled = enabled;

            _logger = logger ?? NullLogger.Instance;
        }

        #endregion

        #region Public Abstract Methods

        public abstract Task ExecuteAsync(CancellationToken cancellationToken);

        #endregion

        #region Private Static Methods

        private static async Task<bool> CanContinueAsync(PeriodicTimer timer, CancellationToken cancellation)
            => await timer.WaitForNextTickAsync(cancellation) && !cancellation.IsCancellationRequested;

        #endregion

        #region Private Methods

        private void DestroyTimer() {
            if (_timer is not null) {
                _timer.Dispose();
                _timer = null;
            }
        }

        #endregion

        #region IHostedService Members

        async Task IHostedService.StartAsync(CancellationToken cancellationToken) {
            _timer ??= new PeriodicTimer(Interval);

            try {
                while (await CanContinueAsync(_timer, cancellationToken)) {
                    await ExecuteAsync(cancellationToken);
                }
            } catch (OperationCanceledException) {
                _logger.LogInformation("Operation cancelled.");
            } catch (Exception ex) {
                _logger.LogError(ex, "{ex.Message}", ex.Message);
            } finally {
                DestroyTimer();
            }
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken) {
            DestroyTimer();

            return Task.CompletedTask;
        }

        #endregion
    }
}
