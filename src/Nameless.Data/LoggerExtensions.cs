using System.Data;
using Microsoft.Extensions.Logging;

namespace Nameless.Data;

internal static class DatabaseLoggerExtensions {
    private static readonly Action<ILogger, string, IDataParameterCollection, Exception?> OutputDbCommandDelegate
        = LoggerMessage.Define<string, IDataParameterCollection>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Command text: {CommandText} | Parameters: {@Params}");

    private static readonly Action<ILogger, Exception> ExecuteNonQueryFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurs while executing non-query command.");

    private static readonly Action<ILogger, Exception> ExecuteScalarFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurs while executing scalar command.");

    private static readonly Action<ILogger, Exception> ExecuteReaderFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurs while executing reader command.");

    extension(ILogger<Database> self) {
        internal void OutputDbCommand(IDbCommand dbCommand) {
            OutputDbCommandDelegate(self, dbCommand.CommandText, dbCommand.Parameters, null /* exception */);
        }

        internal void ExecuteNonQueryFailure(Exception exception) {
            ExecuteNonQueryFailureDelegate(self, exception);
        }

        internal void ExecuteScalarFailure(Exception exception) {
            ExecuteScalarFailureDelegate(self, exception);
        }

        internal void ExecuteReaderFailure(Exception exception) {
            ExecuteReaderFailureDelegate(self, exception);
        }
    }
}