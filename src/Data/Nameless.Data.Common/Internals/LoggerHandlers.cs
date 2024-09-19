using Microsoft.Extensions.Logging;

namespace Nameless.Data.Internals;

internal static class LoggerHandlers {
    internal static readonly Action<ILogger,
        string,
        string,
        Exception?> DebugDbCommand
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Debug,
                                               eventId: default,
                                               formatString: "Command text: {CommandText} | Parameter list: {ParameterList}",
                                               options: null);

    internal static readonly Action<ILogger,
        string,
        Exception?> ErrorOnNonQueryExecution
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "Error while executing non-query. {Message}",
                                       options: null);

    internal static readonly Action<ILogger,
        string,
        Exception?> ErrorOnScalarExecution
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "Error while executing scalar. {Message}",
                                       options: null);

    internal static readonly Action<ILogger,
        string,
        Exception?> ErrorOnReaderExecution
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "Error while executing reader. {Message}",
                                       options: null);
}