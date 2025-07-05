using Microsoft.Extensions.Logging;

namespace Nameless.Web.Infrastructure;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> RecurringTaskErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.RecurringTaskErrorEvent,
            formatString: "Error while executing recurring task.");

    internal static void RecurringTaskError(this ILogger<RecurringHostService> self, Exception exception) {
        RecurringTaskErrorDelegate(self, exception);
    }

    internal static class Events {
        internal static readonly EventId RecurringTaskErrorEvent = new(11003, nameof(RecurringTaskError));
    }
}