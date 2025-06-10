using Microsoft.Extensions.Logging;

namespace Nameless.Web.Services;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> JsonWebTokenValidationFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.JsonWebTokenValidationFailureEvent,
            formatString: "An error occurred while trying to validate the JSON Web Token."
        );

    internal static void JsonWebTokenValidationFailure(this ILogger<JsonWebTokenService> self, Exception exception) {
        JsonWebTokenValidationFailureDelegate(self, exception);
    }

    internal static class Events {
        internal static readonly EventId JsonWebTokenValidationFailureEvent = new(1, nameof(JsonWebTokenValidationFailure));
    }
}
