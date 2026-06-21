namespace Nameless.Microservices.Worker;

public class PrintConsoleWorker : Workers.Worker {
    private readonly ILogger<PrintConsoleWorker> _logger;

    public PrintConsoleWorker(IConfiguration configuration, ILogger<PrintConsoleWorker> logger)
        : base(configuration, logger) {
        _logger = logger;
    }

    public override Task DoWorkAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Hi! I'm printing from a Hosted Service.");

        return Task.CompletedTask;
    }
}
