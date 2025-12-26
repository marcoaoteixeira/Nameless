using Microsoft.Extensions.Logging;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate.Internals;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> ErrorOnSchemaExportExecutionDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.ErrorOnSchemaExportExecutionEvent,
            formatString: "An error occurs while initializing NHibernate.");

    internal static void ErrorOnSchemaExportExecution(this ILogger<SchemaExport> self, Exception exception) {
        ErrorOnSchemaExportExecutionDelegate(self, exception);
    }

    internal static class Events {
        internal static readonly EventId ErrorOnSchemaExportExecutionEvent =
            new(id: 7001, nameof(ErrorOnSchemaExportExecution));
    }
}