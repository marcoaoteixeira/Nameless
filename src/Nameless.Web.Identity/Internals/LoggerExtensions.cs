using Microsoft.Extensions.Logging;
using Nameless.Web.Identity.Domains.UserRefreshTokens;
using Nameless.Web.Identity.UseCases.Authentication.SignIn;
using Nameless.Web.Identity.UseCases.SecurityTokens.AccessToken;
using Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

namespace Nameless.Web.Identity.Internals;

internal static class LoggerExtensions {
    internal static void RefreshTokenIsDisabled(this ILogger<CreateRefreshTokenRequestHandler> self) {
        Delegates.RefreshTokenIsDisabledDelegate(self, null /* exception */);
    }

    internal static void MissingSecretConfiguration(this ILogger<CreateAccessTokenRequestHandler> self) {
        Delegates.MissingSecretConfigurationDelegate(self, null /* exception */);
    }

    internal static void MissingClaimSub(this ILogger<CreateAccessTokenRequestHandler> self) {
        Delegates.MissingClaimSubDelegate(self, null /* exception */);
    }

    internal static void CreateJsonWebTokenFailure(this ILogger<CreateAccessTokenRequestHandler> self, Exception exception) {
        Delegates.CreateJsonWebTokenFailureDelegate(self, exception);
    }

    internal static void SignInUserNotFound(this ILogger<SignInRequestHandler> self, string email) {
        Delegates.SignInUserNotFoundDelegate(self, email, null /* exception */);
    }

    internal static void SignInUserIsLockedOut(this ILogger<SignInRequestHandler> self, string email) {
        Delegates.SignInUserIsLockedOutDelegate(self, email, null /* exception */);
    }

    internal static void SignInUserIsNotAllowed(this ILogger<SignInRequestHandler> self, string email) {
        Delegates.SignInUserIsNotAllowedDelegate(self, email, null /* exception */);
    }

    internal static void CleanUpUserRefreshTokensFailure(this ILogger<UserRefreshTokenManager> self, Exception exception) {
        Delegates.CleanUpUserRefreshTokensFailureDelegate(self, exception);
    }

    internal static void RevokeUserRefreshTokensFailure(this ILogger<UserRefreshTokenManager> self, Exception exception) {
        Delegates.RevokeUserRefreshTokensFailureDelegate(self, exception);
    }

    internal static void CreateUserRefreshTokenFailure(this ILogger<UserRefreshTokenManager> self, Exception exception) {
        Delegates.CreateUserRefreshTokenFailureDelegate(self, exception);
    }

    internal static class Delegates {
        internal static readonly Action<ILogger, Exception?> RefreshTokenIsDisabledDelegate
            = LoggerMessage.Define(
                logLevel: LogLevel.Information,
                eventId: Events.RefreshTokenIsDisabledEvent,
                formatString: "Refresh token is disabled in configuration."
            );

        internal static readonly Action<ILogger, Exception?> MissingSecretConfigurationDelegate
            = LoggerMessage.Define(
                logLevel: LogLevel.Error,
                eventId: Events.MissingSecretConfigurationEvent,
                formatString: "The JSON Web Token (JWT) secret is not configured. Please ensure it is specified in the application settings."
            );

        internal static readonly Action<ILogger, Exception?> MissingClaimSubDelegate
            = LoggerMessage.Define(
                logLevel: LogLevel.Error,
                eventId: Events.MissingClaimSubEvent,
                formatString: "The required claim 'sub' is missing from the claims list."
            );

        internal static readonly Action<ILogger, Exception> CreateJsonWebTokenFailureDelegate
            = LoggerMessage.Define(
                logLevel: LogLevel.Error,
                eventId: Events.CreateJsonWebTokenFailureEvent,
                formatString: "An error occurred while trying to create the JSON Web Token."
            );

        internal static readonly Action<ILogger, string, Exception?> SignInUserNotFoundDelegate
            = LoggerMessage.Define<string>(
                logLevel: LogLevel.Warning,
                eventId: Events.SignInUserNotFoundEvent,
                formatString: "Unable to locate user '{Email}' on sign-in."
            );

        internal static readonly Action<ILogger, string, Exception?> SignInUserIsLockedOutDelegate
            = LoggerMessage.Define<string>(
                logLevel: LogLevel.Warning,
                eventId: Events.SignInUserIsLockedOutEvent,
                formatString: "User '{Email}' is locked out."
            );

        internal static readonly Action<ILogger, string, Exception?> SignInUserIsNotAllowedDelegate
            = LoggerMessage.Define<string>(
                logLevel: LogLevel.Warning,
                eventId: Events.SignInUserIsNotAllowedEvent,
                formatString: "User '{Email}' is not allowed to authenticate."
            );

        internal static readonly Action<ILogger, Exception> CleanUpUserRefreshTokensFailureDelegate
            = LoggerMessage.Define(
                logLevel: LogLevel.Error,
                eventId: Events.CleanUpUserRefreshTokensFailureEvent,
                formatString: "An error occurred while removing previous refresh tokens from the database."
            );

        internal static readonly Action<ILogger, Exception> RevokeUserRefreshTokensFailureDelegate
            = LoggerMessage.Define(
                logLevel: LogLevel.Error,
                eventId: Events.RevokeUserRefreshTokensFailureEvent,
                formatString: "An error occurred while revoking previous refresh tokens from the database."
            );

        internal static readonly Action<ILogger, Exception> CreateUserRefreshTokenFailureDelegate
            = LoggerMessage.Define(
                logLevel: LogLevel.Error,
                eventId: Events.CreateUserRefreshTokenFailureEvent,
                formatString: "An error occurred while creating the refresh tokens."
            );
    }

    internal static class Events {
        internal static readonly EventId RefreshTokenIsDisabledEvent = new(13001, nameof(RefreshTokenIsDisabled));
        internal static readonly EventId MissingSecretConfigurationEvent = new(13002, nameof(MissingSecretConfiguration));
        internal static readonly EventId MissingClaimSubEvent = new(13002, nameof(MissingClaimSub));
        internal static readonly EventId CreateJsonWebTokenFailureEvent = new(13003, nameof(CreateJsonWebTokenFailure));

        internal static readonly EventId SignInUserNotFoundEvent = new(13004, nameof(SignInUserNotFound));
        internal static readonly EventId SignInUserIsLockedOutEvent = new(13005, nameof(SignInUserIsLockedOut));
        internal static readonly EventId SignInUserIsNotAllowedEvent = new(13006, nameof(SignInUserIsNotAllowed));

        internal static readonly EventId CleanUpUserRefreshTokensFailureEvent = new(13007, nameof(CleanUpUserRefreshTokensFailure));
        internal static readonly EventId RevokeUserRefreshTokensFailureEvent = new(13008, nameof(RevokeUserRefreshTokensFailure));
        internal static readonly EventId CreateUserRefreshTokenFailureEvent = new(13009, nameof(CreateUserRefreshTokenFailure));
    }
}

