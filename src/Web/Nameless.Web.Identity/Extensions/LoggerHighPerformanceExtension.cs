using Microsoft.Extensions.Logging;
using Nameless.Web.Identity.Endpoints.Accounts;

namespace Nameless.Web.Identity;

internal static class LoggerHighPerformanceExtension {
    private static readonly Action<ILogger, Exception?> UserSignInSucceededDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Information,
                               eventId: default,
                               formatString: "User signed in.",
                               options: null);

    internal static void UserSignInSucceeded(this ILogger<SignIn> self)
        => UserSignInSucceededDelegate(self, null /* exception */);

    private static readonly Action<ILogger, Exception?> SignInUserIsLockedOutDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Information,
                               eventId: default,
                               formatString: "User is locked out.",
                               options: null);

    internal static void SignInUserIsLockedOut(this ILogger<SignIn> self)
        => SignInUserIsLockedOutDelegate(self, null /* exception */);

    private static readonly Action<ILogger, object, Exception?> TwoFactorAuthSucceededDelegate
        = LoggerMessage.Define<object>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "User with ID '{UserId}' logged in with two-factor authentication.",
                                       options: null);

    internal static void TwoFactorAuthSucceeded(this ILogger<TwoFactorAuth> self, User user)
        => TwoFactorAuthSucceededDelegate(self, user.Id, null /* exception */);

    private static readonly Action<ILogger, object, Exception?> InvalidTwoFactorCodeDelegate
        = LoggerMessage.Define<object>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Invalid authenticator code entered for user with ID '{UserId}'.",
                                       options: null);

    internal static void InvalidTwoFactorCode(this ILogger<TwoFactorAuth> self, User user)
        => InvalidTwoFactorCodeDelegate(self, user.Id, null /* exception */);

    private static readonly Action<ILogger, object, Exception?> TwoFactorUserIsLockedOutDelegate
        = LoggerMessage.Define<object>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "User with ID '{UserId}' account is locked out.",
                                       options: null);

    internal static void TwoFactorUserIsLockedOut(this ILogger<TwoFactorAuth> self, User user)
        => TwoFactorUserIsLockedOutDelegate(self, user.Id, null /* exception */);
}
