using Microsoft.Extensions.Logging;
using Nameless.Web.Identity.Endpoints.Accounts;

namespace Nameless.Web.Identity.Extensions;

internal static class LoggerHighPerformanceExtension {
    private static readonly Action<ILogger, Exception?> UserSignInSucceededDelegate
        = LoggerMessage.Define(LogLevel.Information,
            default,
            "User signed in.",
            null);

    private static readonly Action<ILogger, Exception?> SignInUserIsLockedOutDelegate
        = LoggerMessage.Define(LogLevel.Information,
            default,
            "User is locked out.",
            null);

    private static readonly Action<ILogger, object, Exception?> TwoFactorAuthSucceededDelegate
        = LoggerMessage.Define<object>(LogLevel.Information,
            default,
            "User with ID '{UserId}' logged in with two-factor authentication.",
            null);

    private static readonly Action<ILogger, object, Exception?> InvalidTwoFactorCodeDelegate
        = LoggerMessage.Define<object>(LogLevel.Information,
            default,
            "Invalid authenticator code entered for user with ID '{UserId}'.",
            null);

    private static readonly Action<ILogger, object, Exception?> TwoFactorUserIsLockedOutDelegate
        = LoggerMessage.Define<object>(LogLevel.Information,
            default,
            "User with ID '{UserId}' account is locked out.",
            null);

    internal static void UserSignInSucceeded(this ILogger<SignIn> self) {
        UserSignInSucceededDelegate(self, null /* exception */);
    }

    internal static void SignInUserIsLockedOut(this ILogger<SignIn> self) {
        SignInUserIsLockedOutDelegate(self, null /* exception */);
    }

    internal static void TwoFactorAuthSucceeded(this ILogger<TwoFactorAuth> self, User user) {
        TwoFactorAuthSucceededDelegate(self, user.Id, null /* exception */);
    }

    internal static void InvalidTwoFactorCode(this ILogger<TwoFactorAuth> self, User user) {
        InvalidTwoFactorCodeDelegate(self, user.Id, null /* exception */);
    }

    internal static void TwoFactorUserIsLockedOut(this ILogger<TwoFactorAuth> self, User user) {
        TwoFactorUserIsLockedOutDelegate(self, user.Id, null /* exception */);
    }
}