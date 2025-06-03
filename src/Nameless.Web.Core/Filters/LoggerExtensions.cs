using Microsoft.Extensions.Logging;

namespace Nameless.Web.Filters;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> ValidationServiceUnavailableDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Validation service is unavailable.");

    internal static void ValidationServiceUnavailable(this ILogger<ValidateEndpointFilter> self) {
        ValidationServiceUnavailableDelegate(self, null /* exception */);
    }
}