using Nameless.Microservices.App.Infrastructure;

namespace Nameless.Microservices.App.Internals;

internal static partial class LoggerExtensions {
    private static readonly Action<ILogger, Exception> GlobalExceptionHandlerExecutionDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "GlobalExceptionHandler logged an unhandled exception."
        );

    extension(ILogger<GlobalExceptionHandler> logger) {
        internal void GlobalExceptionHandlerExecution(Exception exception) {
            GlobalExceptionHandlerExecutionDelegate(logger, exception);
        }
    }
}
