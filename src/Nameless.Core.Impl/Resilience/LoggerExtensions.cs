using Microsoft.Extensions.Logging;

namespace Nameless.Resilience;

internal static class LoggerExtensions {
    extension(ILogger<RetryPipelineFactory> self) {
        internal void WarningOnRetry(string? tag, int currentAttempt, int maxAttempts, double delay, Exception? exception) {
            Log.RetryPipelineWriteWarningOnRetry(self, tag, currentAttempt, maxAttempts, delay, exception);
        }
    }
}
