using Microsoft.Extensions.Logging;

namespace Nameless.Bootstrap;

internal static partial class Log {
    private const string TAG = "BOOTSTRAPPER";

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while executing step '{Step}'.")]
    internal static partial void ExecuteStepWithRetryAsyncFailure(ILogger<Bootstrapper> logger, string step, Exception exception, string tag = TAG);
}
