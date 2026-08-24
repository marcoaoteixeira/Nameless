using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.Attributes;
using Nameless.Configuration;
using Nameless.Reporting;

namespace Nameless.Workers;

/// <summary>
///     Represents a worker that executes an action every defined interval.
/// </summary>
public abstract class PeriodicWorker : BackgroundService {
    private readonly IConfiguration _configuration;
    private readonly IStatusReporter<PeriodicWorker> _statusReporter;
    private readonly Lazy<PeriodicWorkerOptions> _options;
    private readonly Lazy<string> _logTag;

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

    private string LogTag => _logTag.Value;

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
    protected PeriodicWorker(IConfiguration configuration, IStatusReporter<PeriodicWorker> statusReporter, ILogger logger) {
        _configuration = configuration;
        _statusReporter = statusReporter;
        Logger = logger;

        _options = new Lazy<PeriodicWorkerOptions>(GetOptions);
        _logTag = new Lazy<string>(() => GetType().Name.ToSnakeCase().ToUpperInvariant());
    }

    /// <inheritdoc />
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken) {
        if (Options.IsDisabled) { return; }

        using var timer = new PeriodicTimer(Options.Interval);
        
        try {
            _statusReporter.Idle(this);

            Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Idle, tag: LogTag);

            while (await timer.WaitForNextTickAsync(stoppingToken)) {
                _statusReporter.Running(this);

                Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Running, tag: LogTag);

                await DoWorkAsync(stoppingToken);

                _statusReporter.Idle(this);

                Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Idle, tag: LogTag);
            }
        }
        catch (OperationCanceledException) {
            _statusReporter.Cancelled(this);

            Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Stopped, tag: LogTag);
            CommonLog.OperationCancelled(Logger, tag: LogTag);
        }
        catch (Exception ex) {
            _statusReporter.Fault(ex);

            Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Faulted, tag: LogTag);
            CommonLog.Failure(Logger, ex, tag: LogTag);

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
                                    .GetOptions<PeriodicWorkerOptions>(Name);

        if (options is null) {
            throw new MissingConfigurationException(
                section: $"{ConfigurationSectionNameAttribute.GetSectionName<PeriodicWorkerOptions>()}:{Name}"
            );
        }

        if (options.Interval <= TimeSpan.Zero) {
            throw new InvalidOperationException(
                "Interval must be greater than zero."
            );
        }

        return options;
    }
}
