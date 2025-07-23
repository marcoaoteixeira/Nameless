using Microsoft.Extensions.Logging;

namespace Nameless.Web.Identity.Security;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> UserRefreshTokenIsDisabledDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Information,
            eventId: Events.UserRefreshTokenIsDisabledEvent,
            formatString: "User refresh token is disabled in configuration."
        );

    internal static void UserRefreshTokenIsDisabled(this ILogger<UserRefreshTokenManager> self) {
        UserRefreshTokenIsDisabledDelegate(self, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId UserRefreshTokenIsDisabledEvent = new(13001, nameof(UserRefreshTokenIsDisabled));
    }
}