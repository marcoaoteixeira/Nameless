using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.Web.Internals;

namespace Nameless.Web.Infrastructure;

public abstract class RecurringHostService : IHostedService, IDisposable {
    private readonly ILogger<RecurringHostService> _logger;

    private IPeriodicTimer? _timer;
    private Task? _executeTask;
    private CancellationTokenSource? _stoppingCts;
    private bool _disposed;

    protected RecurringHostService(IPeriodicTimer timer, ILogger<RecurringHostService> logger) {
        _timer = Guard.Against.Null(timer);
        _logger = Guard.Against.Null(logger);
    }

    ~RecurringHostService() {
        Dispose(disposing: false);
    }

    public abstract Task ExecuteAsync(CancellationToken stoppingToken);

    public Task StartAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executeTask = InnerExecuteAsync(_stoppingCts.Token);

        return _executeTask.IsCompleted ? _executeTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        if (_executeTask is null) {
            return;
        }

        try {
            if (_stoppingCts is not null) {
                await _stoppingCts.CancelAsync();
            }
        }
        finally {
            // Wait until the task completes or the stop token triggers
            var tcs = new TaskCompletionSource<object>();
            await using var registration = cancellationToken.Register(
                state => HandleStopCancellation(state, cancellationToken),
                tcs
            );

            // Do not await the _executeTask because cancelling it will throw
            // an OperationCanceledException which we are explicitly ignoring
            await Task.WhenAny(_executeTask, tcs.Task)
                      .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    public void SetInterval(TimeSpan interval) {
        BlockAccessAfterDispose();

        Guard.Against.LowerOrEqual(interval, TimeSpan.Zero);

        _timer?.Dispose();
        _timer = new PeriodicTimerWrapper(interval);
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private static async Task<bool> CanContinueAsync(IPeriodicTimer timer, CancellationToken cancellationToken) {
        try {
            return !cancellationToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(cancellationToken);
        }
        catch (TaskCanceledException) {
            /* ignore */
        }
        catch (OperationCanceledException) {
            /* ignore */
        }

        return false;
    }

    private static void HandleStopCancellation(object? state, CancellationToken cancellationToken) {
        if (state is TaskCompletionSource<object> tcs) {
            tcs.SetCanceled(cancellationToken);
        }
    }

    private async Task InnerExecuteAsync(CancellationToken stoppingToken) {
        if (_timer is null) { return; }

        while (await CanContinueAsync(_timer, stoppingToken)) {
            try { await ExecuteAsync(stoppingToken); }
            catch (Exception ex) { _logger.RecurringTaskError(ex); }
        }
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, GetType());
    }

    protected virtual void Dispose(bool disposing) {
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
}