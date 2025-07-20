using Microsoft.Extensions.Logging;

namespace Nameless.Web.Identity.Endpoints;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> AuthenticateUserNotFoundDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Unable to locate user '{Email}' on authentication."
        );

    private static readonly Action<ILogger, string, Exception?> UserIsLockedOutDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "User '{Email}' is locked out."
        );

    private static readonly Action<ILogger, string, Exception?> UserIsNotAllowedDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "User '{Email}' is not allowed to authenticate."
        );

    private static readonly Action<ILogger, string, string, Exception?> JwtCreationFailedDelegate
        = LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "JSON Web Token creation for user '{Email}' failed. Error message: {ErrorMessage}."
        );

    internal static void AuthenticateUserNotFound(this ILogger<Authenticate> self, string email) {
        AuthenticateUserNotFoundDelegate(self, email, null /* exception */);
    }

    internal static void UserIsLockedOut(this ILogger<Authenticate> self, string email) {
        UserIsLockedOutDelegate(self, email, null /* exception */);
    }

    internal static void UserIsNotAllowed(this ILogger<Authenticate> self, string email) {
        UserIsNotAllowedDelegate(self, email, null /* exception */);
    }

    internal static void JwtCreationFailed(this ILogger<Authenticate> self, string email, string errorMessage) {
        JwtCreationFailedDelegate(self, email, errorMessage, null /* exception */);
    }
}
