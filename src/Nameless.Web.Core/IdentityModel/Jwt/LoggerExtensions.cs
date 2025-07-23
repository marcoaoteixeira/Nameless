using Microsoft.Extensions.Logging;

namespace Nameless.Web.IdentityModel.Jwt;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> MissingSecretConfigurationDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.MissingSecretConfigurationEvent,
            formatString: "The JSON Web Token (JWT) secret is not configured. Please ensure it is specified in the application settings."
        );

    private static readonly Action<ILogger, Exception?> MissingClaimSubDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.MissingClaimSubEvent,
            formatString: "The required claim 'sub' is missing from the claims list."
        );

    private static readonly Action<ILogger, Exception> CreateJsonWebTokenFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.CreateJsonWebTokenFailureEvent,
            formatString: "An error occurred while trying to create the JSON Web Token."
        );

    internal static void MissingSecretConfiguration(this ILogger<JsonWebTokenProvider> self) {
        MissingSecretConfigurationDelegate(self, null /* exception */);
    }

    internal static void MissingClaimSub(this ILogger<JsonWebTokenProvider> self) {
        MissingClaimSubDelegate(self, null /* exception */);
    }

    internal static void CreateJsonWebTokenFailure(this ILogger<JsonWebTokenProvider> self, Exception exception) {
        CreateJsonWebTokenFailureDelegate(self, exception);
    }

    internal static class Events {
        internal static readonly EventId MissingSecretConfigurationEvent = new(12001, nameof(MissingSecretConfiguration));
        internal static readonly EventId MissingClaimSubEvent = new(12002, nameof(MissingClaimSub));
        internal static readonly EventId CreateJsonWebTokenFailureEvent = new(12003, nameof(CreateJsonWebTokenFailure));
    }
}
