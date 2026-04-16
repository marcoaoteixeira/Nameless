using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nameless.Workers;

public abstract class Worker : BackgroundService {
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;
    private readonly Lazy<WorkerOptions> _options;

    public virtual string Name => GetType().Name;

    private WorkerOptions Options => _options.Value;

    protected Worker(IConfiguration configuration, ILogger<Worker> logger) {
        _configuration = configuration;
        _logger = logger;

        _options = new Lazy<WorkerOptions>(GetOptions);
    }

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken) {
        if (Options.IsDisabled) { return; }

        using var timer = new PeriodicTimer(Options.Interval);

        try {
            while (await timer.WaitForNextTickAsync(stoppingToken)) {
                await DoWorkAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException) { /* ignore */ }
        catch (Exception ex) {
            _logger.Failure(ex);

            throw;
        }
    }

    public abstract Task DoWorkAsync(CancellationToken cancellationToken);

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