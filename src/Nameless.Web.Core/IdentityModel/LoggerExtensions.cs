using Microsoft.Extensions.Logging;

namespace Nameless.Web.IdentityModel;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> CreateSecurityTokenDescriptorFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.CreateSecurityTokenDescriptorFailureEvent,
            formatString: "An error occurred while trying to create the security token descriptor."
        );

    private static readonly Action<ILogger, Exception> CreateJsonWebTokenFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.CreateJsonWebTokenFailureEvent,
            formatString: "An error occurred while trying to create the JSON Web Token."
        );

    internal static void CreateSecurityTokenDescriptorFailure(this ILogger<JsonWebTokenProvider> self, Exception exception) {
        CreateSecurityTokenDescriptorFailureDelegate(self, exception);
    }

    internal static void CreateJsonWebTokenFailure(this ILogger<JsonWebTokenProvider> self, Exception exception) {
        CreateJsonWebTokenFailureDelegate(self, exception);
    }

    internal static class Events {
        internal static readonly EventId CreateSecurityTokenDescriptorFailureEvent = new(12001, nameof(CreateSecurityTokenDescriptorFailure));
        internal static readonly EventId CreateJsonWebTokenFailureEvent = new(12002, nameof(CreateJsonWebTokenFailure));
    }
}
