using Microsoft.Extensions.Logging;

namespace Nameless.WPF.Client.Lucene.DisasterRecovery;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> BackupFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while executing Lucene backup routine."
        );

    extension(ILogger<LuceneDisasterRecoveryRoutine> self) {
        internal void BackupFailure(Exception exception) {
            BackupFailureDelegate(self, exception);
        }
    }
}
