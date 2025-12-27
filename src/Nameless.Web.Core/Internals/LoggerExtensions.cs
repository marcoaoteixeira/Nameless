using Microsoft.Extensions.Logging;
using Nameless.Web.Filters;
using Nameless.Web.Infrastructure;

namespace Nameless.Web.Internals;

internal static class LoggerExtensions {
    internal static void ValidationServiceUnavailable(this ILogger<ValidateRequestEndpointFilter> self) {
        Delegates.ValidationServiceUnavailableDelegate(self, arg2: null /* exception */);
    }

    internal static void RecurringTaskError(this ILogger<RecurringHostService> self, Exception exception) {
        Delegates.RecurringTaskErrorDelegate(self, exception);
    }

    internal static class Delegates {
        internal static readonly Action<ILogger, Exception?> ValidationServiceUnavailableDelegate
            = LoggerMessage.Define(
                LogLevel.Warning,
                Events.ValidationServiceUnavailableEvent,
                formatString: "Validation service is unavailable.");

        internal static readonly Action<ILogger, Exception> RecurringTaskErrorDelegate
            = LoggerMessage.Define(
                LogLevel.Error,
                Events.RecurringTaskErrorEvent,
                formatString: "Error while executing recurring task.");
    }

    internal static class Events {
        internal static readonly EventId ValidationServiceUnavailableEvent =
            new(id: 11002, nameof(ValidationServiceUnavailable));

        internal static readonly EventId RecurringTaskErrorEvent = new(id: 11003, nameof(RecurringTaskError));
    }
}