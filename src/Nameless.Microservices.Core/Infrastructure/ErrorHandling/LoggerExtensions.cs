using Microsoft.Extensions.Logging;

namespace Nameless.Microservices.Infrastructure.ErrorHandling;

internal static class LoggerExtensions {
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
