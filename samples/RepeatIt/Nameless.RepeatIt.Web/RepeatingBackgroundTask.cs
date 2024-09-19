
namespace Nameless.RepeatIt.Web;

public class RepeatingBackgroundTask : BackgroundService {
    private readonly ILogger _logger;
    private readonly PeriodicTimer _timer;

    public RepeatingBackgroundTask(ILogger logger) {
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (await _timer.WaitForNextTickAsync(stoppingToken)) {
            _logger.LogInformation("Repeating...");
        }
    }
}