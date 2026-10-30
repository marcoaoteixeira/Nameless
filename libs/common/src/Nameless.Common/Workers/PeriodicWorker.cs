using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.Configuration;
using Nameless.Contracts;

namespace Nameless.Workers;

/// <summary>
///     Represents a worker that executes an action every defined interval.
/// </summary>
public abstract class PeriodicWorker : BackgroundService {
    private readonly IConfiguration _configuration;
    private readonly Lazy<PeriodicWorkerOptions> _options;

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
    /// <param name="logger">
    ///     The logger.
    /// </param>
    protected PeriodicWorker(IConfiguration configuration, ILogger logger) {
        _configuration = Throws.When.Null(configuration);
        _options = new Lazy<PeriodicWorkerOptions>(GetOptions);
        
        Logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken) {
        if (Options.IsDisabled) { return; }

        var tag = GetType().Tag;

        using var timer = new PeriodicTimer(Options.Interval);
        
        try {
            Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Idle, tag);

            while (await timer.WaitForNextTickAsync(stoppingToken)) {
                Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Running, tag);

                await DoWorkAsync(stoppingToken);

                Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Idle, tag);
            }
        }
        catch (OperationCanceledException ex) {
            Log.StatusChange(Logger, Name, PeriodicWorkerStatus.Stopped, tag);
            CommonLog.Info(Logger, ex.Message, tag);
        }
        catch (Exception ex) {
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

    private PeriodicWorkerOptions GetOptions() {
        var options = _configuration.GetSectionFor<PeriodicWorkerOptions>()
                                    .GetOrThrow<PeriodicWorkerOptions>();

        if (options.Interval <= TimeSpan.Zero) {
            throw new InvalidOperationException(
                "Interval must be greater than zero."
            );
        }

        return options;
    }
}
