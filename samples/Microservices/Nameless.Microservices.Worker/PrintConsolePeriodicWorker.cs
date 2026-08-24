namespace Nameless.Microservices.Worker;

public class PrintConsolePeriodicWorker : Workers.PeriodicWorker {
    private readonly ILogger<PrintConsolePeriodicWorker> _logger;

    public PrintConsolePeriodicWorker(IConfiguration configuration, ILogger<PrintConsolePeriodicWorker> logger)
        : base(configuration, logger) {
        _logger = logger;
    }

    public override Task DoWorkAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Hi! I'm printing from a Hosted Service.");

        return Task.CompletedTask;
    }
}
