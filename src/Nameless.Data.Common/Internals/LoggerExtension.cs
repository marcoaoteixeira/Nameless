using Microsoft.Extensions.Logging;

namespace Nameless.Data.Internals;

internal static class LoggerExtension {
    private static readonly Action<ILogger,
        string,
        string,
        Exception?> OutputDbCommandHandler
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Debug,
                                               eventId: default,
                                               formatString: "Command text: {CommandText} | Parameter list: {ParameterList}",
                                               options: null);

    internal static void OutputDbCommand(this ILogger self, string command, string parameters)
        => OutputDbCommandHandler(self, command, parameters, null /* exception */);

    private static readonly Action<ILogger,
        Exception?> ExecuteNonQueryErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while executing non-query.",
                               options: null);

    internal static void ExecuteNonQueryError(this ILogger self, Exception exception)
        => ExecuteNonQueryErrorHandler(self, exception);

    private static readonly Action<ILogger,
        Exception?> ExecuteScalarErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while executing scalar.",
                               options: null);

    internal static void ExecuteScalarError(this ILogger self, Exception exception)
        => ExecuteScalarErrorHandler(self, exception);

    private static readonly Action<ILogger,
        Exception?> ExecuteReaderErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while executing reader.",
                               options: null);

    internal static void ExecuteReaderError(this ILogger self, Exception exception)
        => ExecuteReaderErrorHandler(self, exception);
}