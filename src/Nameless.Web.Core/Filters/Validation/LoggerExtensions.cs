using Microsoft.Extensions.Logging;

namespace Nameless.Web.Filters.Validation;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> ValidationServiceUnavailableDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Validation service is unavailable."
        );

    extension(ILogger<ValidationFilterBase> self) {
        internal void ValidationServiceUnavailable() {
            ValidationServiceUnavailableDelegate(self, null /* exception */);
        }
    }
}