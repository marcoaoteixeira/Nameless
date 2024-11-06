using Microsoft.Extensions.Logging;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate;

internal static class LoggerHighPerformanceExtension {
    private static readonly Action<ILogger, string, Exception> ErrorOnLoadTypeDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "An error occurs when trying to loading type: {Type}",
                                       options: null);

    internal static void ErrorOnLoadType(this ILogger<ConfigurationFactory> self, string type, Exception exception)
        => ErrorOnLoadTypeDelegate(self, type, exception);

    private static readonly Action<ILogger, string, Exception?> LoadTypeResolveToVoidDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "While trying to loading type it resolve to void: {Type}",
                                       options: null);

    internal static void LoadTypeResolveToVoid(this ILogger<ConfigurationFactory> self, string type)
        => LoadTypeResolveToVoidDelegate(self, type, null /* exception */);

    private static readonly Action<ILogger, Exception> ErrorOnSchemaExportExecutionDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurs while initializing NHibernate.",
                               options: null);

    internal static void ErrorOnSchemaExportExecution(this ILogger<SchemaExport> self, Exception exception)
        => ErrorOnSchemaExportExecutionDelegate(self, exception);
}
