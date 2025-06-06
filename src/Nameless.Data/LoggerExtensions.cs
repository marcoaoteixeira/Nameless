using System.Data;
using Microsoft.Extensions.Logging;

namespace Nameless.Data;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, object, Exception?> OutputDbCommandDelegate
        = LoggerMessage.Define<string, object>(logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Command text: {CommandText} | Parameter list: {@ParameterList}",
            options: null);

    private static readonly Action<ILogger, Exception> ErrorOnExecuteNonQueryCommandDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurs while executing non-query command.",
            options: null);

    private static readonly Action<ILogger, Exception> ErrorOnExecuteScalarErrorDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurs while executing scalar command.",
            options: null);

    private static readonly Action<ILogger, Exception> ErrorOnExecuteReaderCommandDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurs while executing reader command.",
            options: null);

    internal static void OutputDbCommand(this ILogger<Database> self, IDbCommand dbCommand) {
        OutputDbCommandDelegate(self,
            dbCommand.CommandText,
            dbCommand.GetParameterList(),
            null /* exception */);
    }

    internal static void ErrorOnExecuteNonQueryCommand(this ILogger<Database> self, Exception exception) {
        ErrorOnExecuteNonQueryCommandDelegate(self, exception);
    }

    internal static void ErrorOnExecuteScalarCommand(this ILogger<Database> self, Exception exception) {
        ErrorOnExecuteScalarErrorDelegate(self, exception);
    }

    internal static void ErrorOnExecuteReaderCommand(this ILogger<Database> self, Exception exception) {
        ErrorOnExecuteReaderCommandDelegate(self, exception);
    }

    private static object GetParameterList(this IDbCommand self) {
        return self.Parameters
                   .OfType<IDbDataParameter>()
                   .Select(parameter => new {
                       parameter.DbType,
                       parameter.ParameterName,
                       parameter.Value
                   })
                   .ToArray();
    }
}