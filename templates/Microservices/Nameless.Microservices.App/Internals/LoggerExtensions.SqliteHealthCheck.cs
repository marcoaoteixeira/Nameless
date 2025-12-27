using Nameless.Microservices.App.Infrastructure.Sqlite;

namespace Nameless.Microservices.App.Internals;

internal static partial class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> SqliteMissingConnStringDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "Sqlite is not configured. Connection string name: {ConnStringName}"
        );

    private static readonly Action<ILogger, Exception> SqliteConnectionFailedDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "Unable to connect to Sqlite."
        );

    private static readonly Action<ILogger, Exception> SqliteQueryFailedDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "Unable to query Sqlite database."
        );

    extension(ILogger<SqliteHealthCheck> self) {
        internal void SqliteMissingConnString(string connStringName) {
            SqliteMissingConnStringDelegate(self, connStringName, null /* exception */);
        }

        internal void SqliteConnectionFailed(Exception exception) {
            SqliteConnectionFailedDelegate(self, exception);
        }

        internal void SqliteQueryFailed(Exception exception) {
            SqliteQueryFailedDelegate(self, exception);
        }
    }
}
