using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

namespace Nameless.ProducerConsumer.RabbitMQ;

internal static partial class Log {
    internal const string Tag = "RABBIT_PRODUCER_CONSUMER";

    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] Couldn't connect to RabbitMQ broker. Server: {Host}")]
    internal static partial void BrokerUnreachable(ILogger<ConnectionManager> logger, string host, Exception exception, string? tag = Tag);

    [LoggerMessage(level: LogLevel.Debug, message: "[{Tag}] Consumer '{ConsumerTag}' started with reply: {Reply}")]
    internal static partial void ConsumerStarted(ILogger logger, string consumerTag, string? reply, string? tag = Tag);

    [LoggerMessage(level: LogLevel.Debug, message: "[{Tag}] Consumer '{ConsumerTag}' is shutting down with reply: {Reply}")]
    internal static partial void ConsumerShutdown(ILogger logger, string consumerTag, string reply, string? tag = Tag);

    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] Unable to acquire Producer semaphore.")]
    internal static partial void UnableAcquireProducerSemaphore(ILogger logger, Exception exception, string? tag = Tag);
}
