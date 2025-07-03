using Microsoft.Extensions.Logging;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> ErrorOnSchemaExportExecutionDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.ErrorOnSchemaExportExecutionEvent,
            formatString: "An error occurs while initializing NHibernate.");

    internal static void ErrorOnSchemaExportExecution(this ILogger<SchemaExport> self, Exception exception) {
        ErrorOnSchemaExportExecutionDelegate(self, exception);
    }

    internal static class Events {
        internal static readonly EventId ErrorOnSchemaExportExecutionEvent = new(7001, nameof(ErrorOnSchemaExportExecution));
    }
}