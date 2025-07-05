using Microsoft.Extensions.Logging;
using Nameless.Mediator.Requests;
using Nameless.Validation;

namespace Nameless.Barebones.Application.Behaviors;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, string, object, Exception> RequestHandlerUnhandledExceptionDelegate
        = LoggerMessage.Define<string, string, object>(
            logLevel: LogLevel.Error,
            eventId: Events.RequestHandlerUnhandledExceptionEvent,
            formatString: "An error occurred while executing handler for request '{RequestType}' with response '{ResponseType}'. Request data: {@Request}");

    private static readonly Action<ILogger, string, string, object, Exception?> ValidateRequestFailureDelegate
        = LoggerMessage.Define<string, string, object>(
            logLevel: LogLevel.Debug,
            eventId: Events.ValidateRequestFailureEvent,
            formatString: "Validation for request '{RequestType}' with response '{ResponseType}' failed. Validation result: {@ValidationResult}");

    internal static void RequestHandlerUnhandledException<TRequest, TResponse>(this ILogger<UnhandledExceptionPipelineBehavior<TRequest, TResponse>> logger, TRequest request, Exception exception)
        where TRequest : IRequestBase {
        RequestHandlerUnhandledExceptionDelegate(logger,
            typeof(TRequest).GetPrettyName(),
            typeof(TResponse).GetPrettyName(),
            request,
            exception);
    }

    internal static void ValidateRequestFailure<TRequest, TResponse>(this ILogger<ValidateRequestPipelineBehavior<TRequest, TResponse>> logger, ValidationResult result)
        where TRequest : IRequestBase {
        ValidateRequestFailureDelegate(logger,
            typeof(TRequest).GetPrettyName(),
            typeof(TResponse).GetPrettyName(),
            result,
            null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId RequestHandlerUnhandledExceptionEvent = new(1, nameof(RequestHandlerUnhandledException));
        internal static readonly EventId ValidateRequestFailureEvent = new(2, nameof(ValidateRequestFailure));
    }
}
