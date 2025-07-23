using Nameless.Mediator.Requests;
using Nameless.Validation;

namespace Nameless.Microservices.App.PipelineBehaviors;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, string, object, Exception?> ValidateRequestFailureDelegate
        = LoggerMessage.Define<string, string, object>(
            logLevel: LogLevel.Debug,
            eventId: Events.ValidateRequestFailureEvent,
            formatString: "Validation for request '{RequestType}' with response '{ResponseType}' failed. Validation result: {@ValidationResult}");

    internal static void ValidateRequestFailure<TRequest, TResponse>(this ILogger<ValidateRequestPipelineBehavior<TRequest, TResponse>> logger, ValidationResult result)
        where TRequest : IRequestBase {
        ValidateRequestFailureDelegate(logger, typeof(TRequest).GetPrettyName(), typeof(TResponse).GetPrettyName(), result, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId ValidateRequestFailureEvent = new EventId(20002, nameof(ValidateRequestFailure));
    }
}
