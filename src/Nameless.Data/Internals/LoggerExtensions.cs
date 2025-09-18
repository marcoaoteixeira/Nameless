using System.Data;
using Microsoft.Extensions.Logging;

namespace Nameless.Data.Internals;

internal static class DatabaseLoggerExtensions {
    private static readonly Action<ILogger, string, IDataParameterCollection, Exception?> OutputDbCommandDelegate
        = LoggerMessage.Define<string, IDataParameterCollection>(
            logLevel: LogLevel.Debug,
            eventId: Events.OutputDbCommandEvent,
            formatString: "Command text: {CommandText} | Parameters: {@Params}");

    private static readonly Action<ILogger, Exception> ExecuteNonQueryFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.ExecuteNonQueryFailureEvent,
            formatString: "An error occurs while executing non-query command.");

    private static readonly Action<ILogger, Exception> ExecuteScalarFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.ExecuteScalarFailureEvent,
            formatString: "An error occurs while executing scalar command.");

    private static readonly Action<ILogger, Exception> ExecuteReaderFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.ExecuteReaderFailureEvent,
            formatString: "An error occurs while executing reader command.");

    internal static void OutputDbCommand(this ILogger<Database> self, IDbCommand dbCommand) {
        OutputDbCommandDelegate(self, dbCommand.CommandText, dbCommand.Parameters, null /* exception */);
    }

    internal static void ExecuteNonQueryFailure(this ILogger<Database> self, Exception exception) {
        ExecuteNonQueryFailureDelegate(self, exception);
    }

    internal static void ExecuteScalarFailure(this ILogger<Database> self, Exception exception) {
        ExecuteScalarFailureDelegate(self, exception);
    }

    internal static void ExecuteReaderFailure(this ILogger<Database> self, Exception exception) {
        ExecuteReaderFailureDelegate(self, exception);
    }

    internal static class Events {
        internal static readonly EventId OutputDbCommandEvent = new(3001, nameof(OutputDbCommand));
        internal static readonly EventId ExecuteNonQueryFailureEvent = new(3002, nameof(ExecuteNonQueryFailure));
        internal static readonly EventId ExecuteScalarFailureEvent = new(3003, nameof(ExecuteScalarFailure));
        internal static readonly EventId ExecuteReaderFailureEvent = new(3004, nameof(ExecuteReaderFailure));
    }
}