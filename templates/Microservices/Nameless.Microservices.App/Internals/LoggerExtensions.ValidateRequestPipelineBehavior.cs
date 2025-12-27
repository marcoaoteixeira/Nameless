using Nameless.Mediator.Requests;
using Nameless.Microservices.App.Infrastructure.Mediator;
using Nameless.Validation;

namespace Nameless.Microservices.App.Internals;

internal static partial class LoggerExtensions {
    private static readonly Action<ILogger, string, string, object, Exception?> ValidateRequestFailureDelegate
        = LoggerMessage.Define<string, string, object>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Validation for request '{RequestType}' with response '{ResponseType}' failed. Validation result: {@ValidationResult}");

    extension<TRequest, TResponse>(ILogger<ValidateRequestPipelineBehavior<TRequest, TResponse>> logger)
        where TRequest : notnull {
        internal void ValidateRequestFailure(ValidationResult result) {
            ValidateRequestFailureDelegate(logger, typeof(TRequest).GetPrettyName(), typeof(TResponse).GetPrettyName(), result, null /* exception */);
        }
    }
}
