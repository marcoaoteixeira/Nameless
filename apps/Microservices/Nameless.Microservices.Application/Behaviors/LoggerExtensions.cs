using Microsoft.Extensions.Logging;

namespace Nameless.Microservices.Application.Behaviors;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, object, Exception> RequestHandlerUnhandledExceptionDelegate
        = LoggerMessage.Define<string, object>(
            logLevel: LogLevel.Error,
            eventId: Events.RequestHandlerUnhandledExceptionEvent,
            formatString: "Request handler for '{RequestName}' throws unhandled exception. Request: {@Request}");
    internal static void RequestHandlerUnhandledException<TRequest, TResponse>(this ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger, TRequest request, Exception exception)
        where TRequest : notnull {
        RequestHandlerUnhandledExceptionDelegate(logger, request.GetType().Name, request, exception);
    }

    internal static class Events {
        internal static readonly EventId RequestHandlerUnhandledExceptionEvent = new(1, nameof(RequestHandlerUnhandledException));
    }
}
