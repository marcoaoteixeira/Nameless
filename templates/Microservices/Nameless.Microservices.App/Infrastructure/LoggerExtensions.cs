namespace Nameless.Microservices.App.Infrastructure;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> GlobalExceptionHandlerExecutionDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.GlobalExceptionHandlerEvent,
            formatString: "GlobalExceptionHandler logged an unhandled exception."
        );

    internal static void GlobalExceptionHandlerExecution(this ILogger<GlobalExceptionHandler> logger, Exception exception) {
        GlobalExceptionHandlerExecutionDelegate(logger, exception);
    }

    internal static class Events {
        internal static readonly EventId GlobalExceptionHandlerEvent = new EventId(20001, nameof(GlobalExceptionHandler));
    }
}
