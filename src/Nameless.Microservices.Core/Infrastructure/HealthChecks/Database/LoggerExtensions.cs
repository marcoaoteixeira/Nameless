using Microsoft.Extensions.Logging;

namespace Nameless.Microservices.Infrastructure.HealthChecks.Database;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> ConnectionCheckFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to execute the database connection health check."
        );

    private static readonly Action<ILogger, Exception?> MissingSqlCheckDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Missing SQL instruction for database health check, skipping check."
        );

    private static readonly Action<ILogger, Exception> ExecuteSqlFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to execute the database SQL instruction health check."
        );

    private static readonly Action<ILogger, Exception> CreateDbConnectionFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to create the connection to the database."
        );

    extension(ILogger<DatabaseHealthCheck> self) {
        internal void CreateDbConnectionFailure(Exception exception) {
            CreateDbConnectionFailureDelegate(self, exception);
        }

        internal void ConnectionCheckFailure(Exception ex) {
            ConnectionCheckFailureDelegate(self, ex);
        }

        internal void MissingSqlCheck() {
            MissingSqlCheckDelegate(self, null /* exception */);
        }

        internal void ExecuteSqlFailure(Exception ex) {
            ExecuteSqlFailureDelegate(self, ex);
        }
    }
}