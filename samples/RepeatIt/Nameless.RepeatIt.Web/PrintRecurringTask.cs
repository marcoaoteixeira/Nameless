using Nameless.Web.Infrastructure;

namespace Nameless.RepeatIt.Web {
    public class PrintRecurringTask : RecurringTaskHostedService {
        private readonly ILogger<PrintRecurringTask> _logger;

        public PrintRecurringTask(TimeSpan interval, ILogger<PrintRecurringTask> logger)
            : base(interval, logger) {
            _logger = logger;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Print recurring task...");

            return Task.CompletedTask;
        }
    }
}
