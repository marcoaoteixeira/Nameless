using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nameless.Web.Infrastructure;

public abstract class RecurringTaskHostedService : IHostedService, IDisposable {
    private readonly ILogger<RecurringTaskHostedService> _logger;

    private Task? _executeTask;
    private CancellationTokenSource? _stoppingCts;
    private PeriodicTimer? _timer;
    private bool _disposed;

    protected RecurringTaskHostedService(TimeSpan interval, ILogger<RecurringTaskHostedService> logger) {
        Prevent.Argument.LowerOrEqual(interval, TimeSpan.Zero);
        Prevent.Argument.Null(logger);

        _timer = new PeriodicTimer(interval);
        _logger = logger;
    }

    public virtual void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executeTask = InnerExecuteAsync(_stoppingCts.Token);

        return _executeTask.IsCompleted ? _executeTask : Task.CompletedTask;
    }

    async Task IHostedService.StopAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        if (_executeTask is null) {
            return;
        }

        try {
            if (_stoppingCts is not null) {
                await _stoppingCts.CancelAsync();
            }
        } finally {
            // Wait until the task completes or the stop token triggers
            var tcs = new TaskCompletionSource<object>();
            await using var registration =
                cancellationToken.Register(state => HandleStopCancellation(state, cancellationToken),
                                           tcs);

            // Do not await the _executeTask because cancelling it will throw
            // an OperationCanceledException which we are explicitly ignoring
            await Task.WhenAny([_executeTask, tcs.Task])
                      .ConfigureAwait(false);
        }
    }

    public void SetInterval(TimeSpan interval) {
        BlockAccessAfterDispose();

        Prevent.Argument.LowerOrEqual(interval, TimeSpan.Zero);

        if (_timer is not null) {
            _timer.Period = interval;
        }
    }

    public abstract Task ExecuteAsync(CancellationToken stoppingToken);

    private static async Task<bool> CanContinueAsync(PeriodicTimer timer, CancellationToken cancellationToken) {
        try {
            return !cancellationToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(cancellationToken);
        }
        catch (TaskCanceledException) { /* ignore */ }
        catch (OperationCanceledException) { /* ignore */ }

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
}