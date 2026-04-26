using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.Mediator.Events;
using Nameless.Mediator.Pipelines;
using Nameless.Results;
using Nameless.Validation;

namespace Nameless.Mediator;

internal static class LoggerExtensions {
    internal static void MissingEventHandler(this ILogger<EventHandlerWrapper> self, IEvent evt) {
        Log.MediatorMissingEventHandler(self, evt.GetType().GetPrettyName());
    }

    #region ValidateRequestPipelineBehavior<TRequest, TResponse>

    extension<TRequest, TResponse>(ILogger<ValidateRequestPipelineBehavior<TRequest, TResponse>> logger)
        where TRequest : notnull {
        internal void Failure(ValidationResult result) {
            Log.ValidateRequestPipelineBehaviorFailure(
                logger,
                typeof(TRequest).GetPrettyName(),
                reason: JsonSerializer.Serialize(result.Failure ? result.Errors : [])
            );
        }
    }

    #endregion

    #region PerformanceRequestPipelineBehavior<TRequest, TResponse>

    extension<TRequest, TResponse>(ILogger<PerformanceRequestPipelineBehavior<TRequest, TResponse>> logger)
        where TRequest : notnull {
        internal void Starting() {
            Log.PerformanceRequestPipelineBehaviorStarting(logger, typeof(TRequest).GetPrettyName(), typeof(TResponse).GetPrettyName());
        }

        internal void Finished(long elapsedMilliseconds) {
            Log.PerformanceRequestPipelineBehaviorFinished(logger, typeof(TRequest).GetPrettyName(), typeof(TResponse).GetPrettyName(), elapsedMilliseconds);
        }
    }

    #endregion
}