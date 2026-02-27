using Microsoft.Extensions.Logging;
using Nameless.Validation;

namespace Nameless.Microservices.Infrastructure.Mediator;

internal static class LoggerExtensions {
    #region ValidateRequestPipelineBehavior<TRequest, TResponse>

    private static readonly Action<ILogger, string, string, object, Exception?> ValidateRequestFailureDelegate
        = LoggerMessage.Define<string, string, object>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Validation for request '{RequestType}' with response '{ResponseType}' failed. Validation result: {@ValidationResult}"
        );

    extension<TRequest, TResponse>(ILogger<ValidateRequestPipelineBehavior<TRequest, TResponse>> logger)
        where TRequest : notnull {
        internal void ValidateRequestFailure(ValidationResult result) {
            ValidateRequestFailureDelegate(logger, typeof(TRequest).GetPrettyName(), typeof(TResponse).GetPrettyName(), result, null /* exception */);
        }
    }

    #endregion

    #region PerformanceRequestPipelineBehavior<TRequest, TResponse>

    private static readonly Action<ILogger, string, string, Exception?> PerformanceRequestPipelineBehaviorStartingDelegate
        = LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "[PERFORMANCE|START] Request handler: 'IRequestHandler<{RequestType}, {ResponseType}>'"
        );

    private static readonly Action<ILogger, string, string, long, Exception?> PerformanceRequestPipelineBehaviorFinishedDelegate
        = LoggerMessage.Define<string, string, long>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "[PERFORMANCE|FINISH] Request handler: 'IRequestHandler<{RequestType}, {ResponseType}>': {ElapsedMilliseconds}ms."
        );

    extension<TRequest, TResponse>(ILogger<PerformanceRequestPipelineBehavior<TRequest, TResponse>> logger)
        where TRequest : notnull {
        internal void PerformanceRequestPipelineBehaviorStarting() {
            PerformanceRequestPipelineBehaviorStartingDelegate(logger, typeof(TRequest).GetPrettyName(), typeof(TResponse).GetPrettyName(), null /* exception */);
        }

        internal void PerformanceRequestPipelineBehaviorFinished(long elapsedMilliseconds) {
            PerformanceRequestPipelineBehaviorFinishedDelegate(logger, typeof(TRequest).GetPrettyName(), typeof(TResponse).GetPrettyName(), elapsedMilliseconds, null /* exception */);
        }
    }

    #endregion
}