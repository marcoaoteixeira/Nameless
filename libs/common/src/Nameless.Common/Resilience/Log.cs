using Microsoft.Extensions.Logging;

namespace Nameless.Resilience;

internal static partial class Log {
    private const string TAG = "RETRY_PIPELINE_FACTORY";

    [LoggerMessage(level: LogLevel.Warning, message: "[{Tag}] Retrying pipeline '{Pipeline}' due failure. Attempt {CurrentAttempt} of {MaxAttempts}. Waiting delay of {Delay}ms before retry.")]
    internal static partial void WarningOnRetry(ILogger<RetryPipelineFactory> logger, string? pipeline, int currentAttempt, int maxAttempts, double delay, Exception? exception = null, string? tag = TAG);
}
