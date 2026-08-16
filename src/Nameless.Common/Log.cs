using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Data;
using Nameless.Diagnostics.CodeAnalysis;
using Nameless.Mailing.Mailkit;
using Nameless.Mediator.Events;
using Nameless.ProducerConsumer.RabbitMQ;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.Resilience;
using Nameless.Workers;

namespace Nameless;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGen)]
internal static partial class Log {
    #region Common

    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] An error has occurred while executing action '{ActionName}'.")]
    internal static partial void Failure(ILogger logger, string tag, string actionName, Exception exception);

    [LoggerMessage(level: LogLevel.Warning, message: "[{Tag}] A problem has occurred while executing action '{ActionName}'. Reason: {Reason}")]
    internal static partial void Warning(ILogger logger, string tag, string actionName, string reason);

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

    #region Workers

    [LoggerMessage(level: LogLevel.Information, message: "[{Tag}] Worker '{WorkerName}' status changed to '{Status}'.")]
    internal static partial void WorkerStatusChanged(ILogger logger, string tag, string workerName, WorkerStatus status);

    #endregion
}
