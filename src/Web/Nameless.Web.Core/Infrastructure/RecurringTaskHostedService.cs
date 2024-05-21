using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Web.Infrastructure {
    public abstract class RecurringTaskHostedService : IHostedService, IDisposable {
        #region Private Read-Only Fields

        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private Task? _executeTask;
        private PeriodicTimer? _timer;
        private CancellationTokenSource? _stoppingCts;

        private bool _disposed;

        #endregion

        #region Protected Constructors

        protected RecurringTaskHostedService(TimeSpan interval)
            : this(interval, NullLogger<RecurringTaskHostedService>.Instance) { }

        protected RecurringTaskHostedService(TimeSpan interval, ILogger<RecurringTaskHostedService> logger) {
            Guard.Against.LowerThanZero(interval, nameof(interval));
            Guard.Against.Null(logger, nameof(logger));

            _timer = new PeriodicTimer(interval);
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public void SetInterval(TimeSpan interval) {
            BlockAccessAfterDispose();

            Guard.Against.LowerThanZero(interval, nameof(interval));

            if (_timer is not null) {
                _timer.Period = interval;
            }
        }

        #endregion

        #region Public Abstract Methods

        public abstract Task ExecuteAsync(CancellationToken stoppingToken);

        #endregion

        #region Private Static Methods

        private static async Task<bool> ContinueAsync(PeriodicTimer timer, CancellationToken cancellationToken)
            => await timer.WaitForNextTickAsync(cancellationToken) &&
                !cancellationToken.IsCancellationRequested;

        #endregion

        #region Private Methods

        private async Task InnerExecuteAsync(CancellationToken stoppingToken) {
            if (_timer is null) { return; }

            while (await ContinueAsync(_timer, stoppingToken)) {
                try { await ExecuteAsync(stoppingToken); }
                catch (Exception ex) {
                    _logger.LogError(
                        exception: ex,
                        message: "Error while executing recurring task: {Message}",
                        args: ex.Message
                    );
                }
            }
        }

        private void BlockAccessAfterDispose()
            => ObjectDisposedException.ThrowIf(_disposed, GetType());

        private void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }

            if (disposing) {
                _timer?.Dispose();

                _stoppingCts?.Cancel();
                _stoppingCts?.Dispose();
            }

            _timer = null;
            _stoppingCts = null;
            _disposed = true;
        }

        #endregion

        #region IHostedService Members

        Task IHostedService.StartAsync(CancellationToken cancellationToken) {
            BlockAccessAfterDispose();

            _stoppingCts = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken);

            _executeTask = InnerExecuteAsync(_stoppingCts.Token);

            return _executeTask.IsCompleted
                ? _executeTask
                : Task.CompletedTask;
        }

        async Task IHostedService.StopAsync(CancellationToken cancellationToken) {
            BlockAccessAfterDispose();

            if (_executeTask is null) {
                return;
            }

            try { _stoppingCts?.Cancel(); } finally {
                // Wait until the task completes or the stop token triggers
                var taskCompletionSource = new TaskCompletionSource<object>();
                using var registration = cancellationToken.Register(
                    callback: state => {
                        if (state is TaskCompletionSource<object> tcs) {
                            tcs.SetCanceled();
                        }
                    },
                    state: taskCompletionSource
                );
                // Do not await the _executeTask because cancelling it will throw
                // an OperationCanceledException which we are explicitly ignoring
                await Task
                    .WhenAny([_executeTask, taskCompletionSource.Task])
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
