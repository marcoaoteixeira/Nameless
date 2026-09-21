using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.Reporting;

namespace Nameless.Workers;

/// <summary>
///     Represents a worker that executes an action every defined interval.
/// </summary>
public abstract class PeriodicWorker : BackgroundService {
    private readonly IConfiguration _configuration;
    private readonly IStatusReporter _statusReporter;
    private readonly Lazy<PeriodicWorkerOptions> _options;

    private bool _disposed;

    /// <summary>
    ///     Gets the worker name.
    /// </summary>
    public virtual string Name => GetType().Name;

    /// <summary>
    ///     Gets the current instance of <see cref="ILogger"/>.
    /// </summary>
    protected ILogger Logger { get; }

    private PeriodicWorkerOptions Options => _options.Value;

    /// <summary>
    ///     Initializes a new instance of <see cref="PeriodicWorker"/> class.
    /// </summary>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <param name="statusReporter">
    ///     The status reporter.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    protected PeriodicWorker(IConfiguration configuration, IStatusReporter statusReporter, ILogger logger) {
        _configuration = configuration;
        _statusReporter = statusReporter;
        Logger = logger;

        _options = new Lazy<PeriodicWorkerOptions>(GetOptions);
    }

    /// <inheritdoc />
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken) {
        if (Options.IsDisabled) { return; }

        var tag = GetType().Tag;

        using var timer = new PeriodicTimer(Options.Interval);
        
        try {
            _statusReporter.Idle(this);

            Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Idle, tag);

            while (await timer.WaitForNextTickAsync(stoppingToken)) {
                _statusReporter.Running(this);

                Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Running, tag);

                await DoWorkAsync(stoppingToken);

                _statusReporter.Idle(this);

                Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Idle, tag);
            }
        }
        catch (OperationCanceledException ex) {
            _statusReporter.Cancelled(this);

            Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Stopped, tag);
            CommonLog.Info(Logger, ex.Message, tag);
        }
        catch (Exception ex) {
            _statusReporter.Fault(ex);

            Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Faulted, tag);
            CommonLog.Error(Logger, ex.Message, ex, tag);

            throw;
        }
    }

    /// <summary>
    ///     Executes the work every defined interval.
    /// </summary>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous execution.
    /// </returns>
    public abstract Task DoWorkAsync(CancellationToken cancellationToken);
    
    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken) {
        await base.StopAsync(cancellationToken);

        _statusReporter.Stop(this);
        _statusReporter.Complete();
    }

    /// <inheritdoc />
    public override void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);

        base.Dispose();
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        _disposed = true;

        if (!disposing) { return; }

        if (_statusReporter is IDisposable disposable) {
            disposable.Dispose();
        }
    }

    private PeriodicWorkerOptions GetOptions() {
        var options = _configuration.GetSection<PeriodicWorkerOptions>()
                                    .GetOrThrow<PeriodicWorkerOptions>();

        if (options.Interval <= TimeSpan.Zero) {
            throw new InvalidOperationException(
                "Interval must be greater than zero."
            );
        }

        return options;
    }
}
