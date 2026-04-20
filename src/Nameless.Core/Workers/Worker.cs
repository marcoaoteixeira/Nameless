using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.Workers.Notification;

namespace Nameless.Workers;

/// <summary>
///     Represents a worker that executes an action every defined interval.
/// </summary>
public abstract class Worker : BackgroundService {
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;
    private readonly Lazy<WorkerOptions> _options;
    private readonly WorkerProgressSubject _subject = new();

    private volatile int _status = (int)WorkerStatus.Idle;

    private bool _disposed;

    /// <summary>
    ///     Gets the worker name.
    /// </summary>
    public virtual string Name => GetType().Name;

    /// <summary>
    ///     Gets the current execution status of this worker.
    ///     Safe to read from any thread.
    /// </summary>
    public WorkerStatus Status => (WorkerStatus)_status;

    /// <summary>
    ///     Gets an observable stream of <see cref="WorkerProgress" />
    ///     notifications pushed during each execution cycle.
    ///     Subscribe before the worker starts to receive all events.
    /// </summary>
    public IObservable<WorkerProgress> Progress => _subject;

    /// <summary>
    ///     Gets the most recently published <see cref="WorkerProgress" />,
    ///     or <see langword="null" /> if no progress has been reported yet.
    ///     Suitable for polling scenarios such as health-check endpoints.
    /// </summary>
    public WorkerProgress? LastProgress => _subject.LastProgress;

    private WorkerOptions Options => _options.Value;

    /// <summary>
    ///     Initializes a new instance of <see cref="Worker"/> class.
    /// </summary>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    protected Worker(IConfiguration configuration, ILogger<Worker> logger) {
        _configuration = configuration;
        _logger = logger;

        _options = new Lazy<WorkerOptions>(GetOptions);
    }

    /// <inheritdoc />
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken) {
        if (Options.IsDisabled) { return; }

        using var timer = new PeriodicTimer(Options.Interval);
        SetStatus(WorkerStatus.Idle);

        try {
            while (await timer.WaitForNextTickAsync(stoppingToken)) {
                SetStatus(WorkerStatus.Running);
                _logger.StatusChanged(Name, WorkerStatus.Running);

                await DoWorkAsync(stoppingToken);

                SetStatus(WorkerStatus.Idle);
                _logger.StatusChanged(Name, WorkerStatus.Idle);
            }
        }
        catch (OperationCanceledException) {
            SetStatus(WorkerStatus.Stopped);
            _logger.StatusChanged(Name, WorkerStatus.Stopped);
        }
        catch (Exception ex) {
            SetStatus(WorkerStatus.Faulted);
            _logger.StatusChanged(Name, WorkerStatus.Faulted);
            _logger.Failure(ex);

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

    /// <summary>
    ///     Publishes a <see cref="WorkerProgress" /> notification to all
    ///     current subscribers and updates <see cref="LastProgress" />.
    /// </summary>
    /// <param name="progress">The progress notification to publish.</param>
    public void ReportProgress(WorkerProgress progress) {
        _subject.OnNext(progress);
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken) {
        await base.StopAsync(cancellationToken);

        SetStatus(WorkerStatus.Stopped);

        _subject.OnCompleted();
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

        if (disposing) {
            _subject.Dispose();
        }
    }

    private void SetStatus(WorkerStatus status) {
        _status = (int)status;
    }

    private WorkerOptions GetOptions() {
        var options = _configuration.GetMultipleOptions<WorkerOptions>()
                                    .GetValueOrDefault(Name);

        if (options is null) {
            throw new InvalidOperationException(
                $"Missing configuration for Worker '{Name}'."
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
