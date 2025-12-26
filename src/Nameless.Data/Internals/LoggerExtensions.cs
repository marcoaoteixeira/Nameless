using System.Data;
using Microsoft.Extensions.Logging;

namespace Nameless.Data.Internals;

internal static class DatabaseLoggerExtensions {
    private static readonly Action<ILogger, string, IDataParameterCollection, Exception?> OutputDbCommandDelegate
        = LoggerMessage.Define<string, IDataParameterCollection>(
            LogLevel.Debug,
            Events.OutputDbCommandEvent,
            formatString: "Command text: {CommandText} | Parameters: {@Params}");

    private static readonly Action<ILogger, Exception> ExecuteNonQueryFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.ExecuteNonQueryFailureEvent,
            formatString: "An error occurs while executing non-query command.");

    private static readonly Action<ILogger, Exception> ExecuteScalarFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.ExecuteScalarFailureEvent,
            formatString: "An error occurs while executing scalar command.");

    private static readonly Action<ILogger, Exception> ExecuteReaderFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.ExecuteReaderFailureEvent,
            formatString: "An error occurs while executing reader command.");

    extension(ILogger<Database> self) {
        internal void OutputDbCommand(IDbCommand dbCommand) {
            OutputDbCommandDelegate(self, dbCommand.CommandText, dbCommand.Parameters, arg4: null /* exception */);
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

    internal static class Events {
        internal static readonly EventId OutputDbCommandEvent = new(id: 3001, nameof(OutputDbCommand));
        internal static readonly EventId ExecuteNonQueryFailureEvent = new(id: 3002, nameof(ExecuteNonQueryFailure));
        internal static readonly EventId ExecuteScalarFailureEvent = new(id: 3003, nameof(ExecuteScalarFailure));
        internal static readonly EventId ExecuteReaderFailureEvent = new(id: 3004, nameof(ExecuteReaderFailure));
    }
}