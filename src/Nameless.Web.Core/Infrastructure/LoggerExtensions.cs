using Microsoft.Extensions.Logging;

namespace Nameless.Web.Infrastructure;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> RecurringTaskErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "Error while executing recurring task.");

    internal static void RecurringTaskError(this ILogger<RecurringTaskHostedService> self, Exception exception) {
        RecurringTaskErrorDelegate(self, exception);
    }
}