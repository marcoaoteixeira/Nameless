using Microsoft.Extensions.Logging;

namespace Nameless.Workers;

internal static partial class Log {
    [LoggerMessage(level: LogLevel.Information, message: "[{Tag}] Worker '{Name}' status change: {CurrentStatus}")]
    internal static partial void StatusChange(ILogger logger, string name, PeriodicWorkerStatus currentStatus, string? tag = null);
}
