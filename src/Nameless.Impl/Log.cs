using Microsoft.Extensions.Logging;
using Nameless.Bootstrap;
using Nameless.Data;
using Nameless.Mailing.Mailkit;
using Nameless.Mediator.Events;
using Nameless.ProducerConsumer.RabbitMQ;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.Resilience;

namespace Nameless;

internal static partial class Log {
    #region Common

    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] An error has occurred while executing action '{ActionName}'.")]
    internal static partial void Failure(ILogger logger, string tag, string actionName, Exception exception);

    #endregion

    #region Data

    [LoggerMessage(level: LogLevel.Debug, message: "[{Tag}] Command text: {CommandText} | Parameters: {@Parameters}")]
    internal static partial void DatabaseOutputDbCommand(ILogger<Database> logger, string tag, string commandText, object parameters);

    #endregion

    #region Mailing

    [LoggerMessage(level: LogLevel.Debug, message: "The message was delivered with result: {Result}")]
    internal static partial void MailingDeliverResult(ILogger<MailingService> logger, string result);

    #endregion

    #region Mediator

    [LoggerMessage(level: LogLevel.Debug, message: "There are no event handler for event type '{EventType}'")]
    internal static partial void MediatorMissingEventHandler(ILogger<EventHandlerWrapper> logger, string eventType);

    #endregion

    #region Resilience

    [LoggerMessage(level: LogLevel.Warning, message: "[{Tag}] Retrying due failure. Attempt {CurrentAttempt} of {MaxAttempts}. Waiting delay of {Delay}ms before retry.")]
    internal static partial void RetryPipelineWriteWarningOnRetry(ILogger<RetryPipelineFactory> logger, string? tag, int currentAttempt, int maxAttempts, double delay, Exception? exception);

    #endregion

    #region Bootstrap

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Starting Bootstrapper with {TotalSteps} available steps...")]
    internal static partial void BootstrapStarting(ILogger<Bootstrapper> logger, int totalSteps);

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Step dependency graph built: {LevelCount} execution levels with a total of {StepCount} steps.")]
    internal static partial void BootstrapStepDependencyGraphBuilt(ILogger<Bootstrapper> logger, int levelCount, int stepCount);

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Executing Bootstrap in '{Mode}' mode.")]
    internal static partial void BootstrapExecutionMode(ILogger<Bootstrapper> logger, string mode);

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Bootstrapper took {ElapsedMilliseconds}ms to complete with {SuccessCount}/{TotalSteps} steps executed.")]
    internal static partial void BootstrapFinished(ILogger<Bootstrapper> logger, long elapsedMilliseconds, int successCount, int totalSteps);

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Execution Statistics => Mean: {Avg:F2}ms, Max: {Max:F2}ms, Min: {Min:F2}ms")]
    internal static partial void BootstrapWriteExecutionStatistics(ILogger<Bootstrapper> logger, double avg, double max, double min);

    #endregion

    #region Bootstrap - Step

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Now executing step {CurrentStep} of {TotalSteps}: '{StepName}'")]
    internal static partial void BootstrapCurrentlyExecutingStep(ILogger<Bootstrapper> logger, int currentStep, int totalSteps, string stepName);

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Step '{StepName}' starting...")]
    internal static partial void BootstrapStepStarting(ILogger<Bootstrapper> logger, string stepName);

    [LoggerMessage(level: LogLevel.Information, message: "[BOOTSTRAP] Step '{StepName}' will not execute since it is disabled.")]
    internal static partial void BootstrapStepDisabled(ILogger<Bootstrapper> logger, string stepName);

    [LoggerMessage(level: LogLevel.Error, message: "[BOOTSTRAP] An error has occurred while executing step '{StepName}'.")]
    internal static partial void BootstrapStepFailure(ILogger<Bootstrapper> logger, string stepName, Exception exception);
    
    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Step '{StepName}' finished. Execution took {ElapsedMilliseconds}ms to complete.")]
    internal static partial void BootstrapStepFinished(ILogger<Bootstrapper> logger, string stepName, long elapsedMilliseconds);

    #endregion

    #region Bootstrap (Parallel Mode)

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Executing step level {CurrentLevel} of {LevelCount} with {StepCount} step(s).")]
    internal static partial void BootstrapExecutingStepInLevel(ILogger<ParallelBootstrapper> logger, int currentLevel, int levelCount, int stepCount);

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Executing single step in the current level: '{StepName}'")]
    internal static partial void BootstrapExecutingSingleStepInLevel(ILogger<ParallelBootstrapper> logger, string stepName);

    [LoggerMessage(level: LogLevel.Debug, message: "[BOOTSTRAP] Executing {StepCount} steps in the current level: '{StepNames}'")]
    internal static partial void BootstrapExecutingMultipleStepInLevel(ILogger<ParallelBootstrapper> logger, int stepCount, string stepNames);

    #endregion

    #region Producer/Consumer

    [LoggerMessage(level: LogLevel.Debug, message: "[PRODUCER/CONSUMER] Consumer '{ConsumerTag}' is shutting down with reply: {Reply}")]
    internal static partial void ConsumerShutdown(ILogger logger, string consumerTag, string reply);

    [LoggerMessage(level: LogLevel.Error, message: "[PRODUCER/CONSUMER] Consumer failed to deserialize Envelope.")]
    internal static partial void ConsumerDeserializeEnvelopeFailure(ILogger logger, Exception exception);

    [LoggerMessage(level: LogLevel.Error, message: "[PRODUCER/CONSUMER] Consumer failed to deserialize message.")]
    internal static partial void ConsumerDeserializeMessageFailure(ILogger logger, Exception exception);

    [LoggerMessage(level: LogLevel.Debug, message: "[PRODUCER/CONSUMER] Consumer '{ConsumerTag}' started with reply: {Reply}")]
    internal static partial void ConsumerStarted(ILogger logger, string consumerTag, string? reply);

    [LoggerMessage(level: LogLevel.Error, message: "[PRODUCER/CONSUMER] Couldn't connect to RabbitMQ broker. Server: {ServerInfo}")]
    internal static partial void ConnectionManagerBrokerUnreachable(ILogger<ConnectionManager> logger, string serverInfo, Exception exception);

    [LoggerMessage(level: LogLevel.Warning, message: "[PRODUCER/CONSUMER] Channel semaphore was disposed due racing condition between FetchCacheEntryAsync and InnerProduceAsync.")]
    internal static partial void ProducerChannelSemaphoreDisposed(ILogger<Producer> logger);

    #endregion

    #region PerformanceRequestPipelineBehavior<TRequest, TResponse>

    [LoggerMessage(level: LogLevel.Debug, message: "[PERFORMANCE] 'IRequestHandler<{RequestType}, {ResponseType}>' starting.")]
    internal static partial void PerformanceRequestPipelineBehaviorStarting(ILogger logger, string requestType, string responseType);

    [LoggerMessage(level: LogLevel.Debug, message: "[PERFORMANCE] 'IRequestHandler<{RequestType}, {ResponseType}>' finished execution in {ElapsedMilliseconds}ms.")]
    internal static partial void PerformanceRequestPipelineBehaviorFinished(ILogger logger, string requestType, string responseType, long elapsedMilliseconds);

    #endregion

    #region ValidateRequestPipelineBehavior<TRequest, TResponse>

    [LoggerMessage(level: LogLevel.Information, message: "[VALIDATION] '{RequestType}' failed validation step. Reason: {Reason}")]
    internal static partial void ValidateRequestPipelineBehaviorFailure(ILogger logger, string requestType, string reason);

    #endregion
}
