using Microsoft.Extensions.Logging;

namespace Nameless.Web.Filters;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> ValidationServiceUnavailableDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Warning,
            eventId: Events.ValidationServiceUnavailableEvent,
            formatString: "Validation service is unavailable.");

    internal static void ValidationServiceUnavailable(this ILogger<ValidateRequestEndpointFilter> self) {
        ValidationServiceUnavailableDelegate(self, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId ValidationServiceUnavailableEvent = new(11002, nameof(ValidationServiceUnavailable));
    }
}