using Microsoft.Extensions.Logging;

namespace Nameless.WPF.Client.Sqlite.DisasterRecovery;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> BackupFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while executing SQLite disaster recovery routine."
        );

    extension(ILogger<SqliteDisasterRecoveryRoutine> self) {
        internal void BackupFailure(Exception exception) {
            BackupFailureDelegate(self, exception);
        }
    }
}
