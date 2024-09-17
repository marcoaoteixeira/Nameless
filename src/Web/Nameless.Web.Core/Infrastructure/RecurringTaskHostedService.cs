using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Web.Internals;

namespace Nameless.Web.Infrastructure;

public abstract class RecurringTaskHostedService : IHostedService, IDisposable {
    private readonly ILogger _logger;

    private Task? _executeTask;
    private PeriodicTimer? _timer;
    private CancellationTokenSource? _stoppingCts;

    private bool _disposed;

    protected RecurringTaskHostedService(TimeSpan interval)
        : this(interval, NullLogger<RecurringTaskHostedService>.Instance) { }

    protected RecurringTaskHostedService(TimeSpan interval, ILogger<RecurringTaskHostedService> logger) {
        Prevent.Argument.LowerOrEqual(interval, to: TimeSpan.Zero);
        Prevent.Argument.Null(logger);

        _timer = new PeriodicTimer(interval);
        _logger = logger;
    }

    public void SetInterval(TimeSpan interval) {
        BlockAccessAfterDispose();

        Prevent.Argument.LowerOrEqual(interval, to: TimeSpan.Zero);

        if (_timer is not null) {
            _timer.Period = interval;
        }
    }

    public abstract Task ExecuteAsync(CancellationToken stoppingToken);

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
        }
        finally {
            // Wait until the task completes or the stop token triggers
            var tcs = new TaskCompletionSource<object>();
            await using var registration = cancellationToken.Register(callback: state => HandleStopCancellation(state, cancellationToken),
                                                                      state: tcs);

            // Do not await the _executeTask because cancelling it will throw
            // an OperationCanceledException which we are explicitly ignoring
            await Task.WhenAny([_executeTask, tcs.Task])
                      .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    public virtual void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private static async Task<bool> ContinueAsync(PeriodicTimer timer, CancellationToken cancellationToken)
        => await timer.WaitForNextTickAsync(cancellationToken) &&
           !cancellationToken.IsCancellationRequested;

    private static void HandleStopCancellation(object? state, CancellationToken cancellationToken) {
        if (state is TaskCompletionSource<object> tcs) {
            tcs.SetCanceled(cancellationToken);
        }
    }

    private async Task InnerExecuteAsync(CancellationToken stoppingToken) {
        if (_timer is null) { return; }

        while (await ContinueAsync(_timer, stoppingToken)) {
            try { await ExecuteAsync(stoppingToken); }
            catch (Exception ex) { LoggerHandlers.RecurringTaskException(_logger, ex); }
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