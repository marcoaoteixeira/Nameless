using Microsoft.Extensions.Logging;
using Nameless.Web.Identity.Domains.UserRefreshTokens;
using Nameless.Web.Identity.UseCases.Authentication.SignIn;
using Nameless.Web.Identity.UseCases.SecurityTokens.AccessToken;
using Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

namespace Nameless.Web.Identity.Internals;

internal static class LoggerExtensions {
    internal static void RefreshTokenIsDisabled(this ILogger<CreateRefreshTokenRequestHandler> self) {
        Delegates.RefreshTokenIsDisabledDelegate(self, arg2: null /* exception */);
    }

    extension(ILogger<CreateAccessTokenRequestHandler> self) {
        internal void MissingSecretConfiguration() {
            Delegates.MissingSecretConfigurationDelegate(self, arg2: null /* exception */);
        }

        internal void MissingClaimSub() {
            Delegates.MissingClaimSubDelegate(self, arg2: null /* exception */);
        }

        internal void CreateJsonWebTokenFailure(Exception exception) {
            Delegates.CreateJsonWebTokenFailureDelegate(self, exception);
        }
    }

    extension(ILogger<SignInRequestHandler> self) {
        internal void SignInUserNotFound(string email) {
            Delegates.SignInUserNotFoundDelegate(self, email, arg3: null /* exception */);
        }

        internal void SignInUserIsLockedOut(string email) {
            Delegates.SignInUserIsLockedOutDelegate(self, email, arg3: null /* exception */);
        }

        internal void SignInUserIsNotAllowed(string email) {
            Delegates.SignInUserIsNotAllowedDelegate(self, email, arg3: null /* exception */);
        }
    }

    extension(ILogger<UserRefreshTokenManager> self) {
        internal void CleanUpUserRefreshTokensFailure(Exception exception) {
            Delegates.CleanUpUserRefreshTokensFailureDelegate(self, exception);
        }

        internal void RevokeUserRefreshTokensFailure(Exception exception) {
            Delegates.RevokeUserRefreshTokensFailureDelegate(self, exception);
        }

        internal void CreateUserRefreshTokenFailure(Exception exception) {
            Delegates.CreateUserRefreshTokenFailureDelegate(self, exception);
        }
    }

    internal static class Delegates {
        internal static readonly Action<ILogger, Exception?> RefreshTokenIsDisabledDelegate
            = LoggerMessage.Define(
                LogLevel.Information,
                Events.RefreshTokenIsDisabledEvent,
                formatString: "Refresh token is disabled in configuration."
            );

        internal static readonly Action<ILogger, Exception?> MissingSecretConfigurationDelegate
            = LoggerMessage.Define(
                LogLevel.Error,
                Events.MissingSecretConfigurationEvent,
                formatString:
                "The JSON Web Token (JWT) secret is not configured. Please ensure it is specified in the application settings."
            );

        internal static readonly Action<ILogger, Exception?> MissingClaimSubDelegate
            = LoggerMessage.Define(
                LogLevel.Error,
                Events.MissingClaimSubEvent,
                formatString: "The required claim 'sub' is missing from the claims list."
            );

        internal static readonly Action<ILogger, Exception> CreateJsonWebTokenFailureDelegate
            = LoggerMessage.Define(
                LogLevel.Error,
                Events.CreateJsonWebTokenFailureEvent,
                formatString: "An error occurred while trying to create the JSON Web Token."
            );

        internal static readonly Action<ILogger, string, Exception?> SignInUserNotFoundDelegate
            = LoggerMessage.Define<string>(
                LogLevel.Warning,
                Events.SignInUserNotFoundEvent,
                formatString: "Unable to locate user '{Email}' on sign-in."
            );

        internal static readonly Action<ILogger, string, Exception?> SignInUserIsLockedOutDelegate
            = LoggerMessage.Define<string>(
                LogLevel.Warning,
                Events.SignInUserIsLockedOutEvent,
                formatString: "User '{Email}' is locked out."
            );

        internal static readonly Action<ILogger, string, Exception?> SignInUserIsNotAllowedDelegate
            = LoggerMessage.Define<string>(
                LogLevel.Warning,
                Events.SignInUserIsNotAllowedEvent,
                formatString: "User '{Email}' is not allowed to authenticate."
            );

        internal static readonly Action<ILogger, Exception> CleanUpUserRefreshTokensFailureDelegate
            = LoggerMessage.Define(
                LogLevel.Error,
                Events.CleanUpUserRefreshTokensFailureEvent,
                formatString: "An error occurred while removing previous refresh tokens from the database."
            );

        internal static readonly Action<ILogger, Exception> RevokeUserRefreshTokensFailureDelegate
            = LoggerMessage.Define(
                LogLevel.Error,
                Events.RevokeUserRefreshTokensFailureEvent,
                formatString: "An error occurred while revoking previous refresh tokens from the database."
            );

        internal static readonly Action<ILogger, Exception> CreateUserRefreshTokenFailureDelegate
            = LoggerMessage.Define(
                LogLevel.Error,
                Events.CreateUserRefreshTokenFailureEvent,
                formatString: "An error occurred while creating the refresh tokens."
            );
    }

    internal static class Events {
        internal static readonly EventId RefreshTokenIsDisabledEvent = new(id: 13001, nameof(RefreshTokenIsDisabled));

        internal static readonly EventId MissingSecretConfigurationEvent =
            new(id: 13002, nameof(MissingSecretConfiguration));

        internal static readonly EventId MissingClaimSubEvent = new(id: 13002, nameof(MissingClaimSub));

        internal static readonly EventId CreateJsonWebTokenFailureEvent =
            new(id: 13003, nameof(CreateJsonWebTokenFailure));

        internal static readonly EventId SignInUserNotFoundEvent = new(id: 13004, nameof(SignInUserNotFound));
        internal static readonly EventId SignInUserIsLockedOutEvent = new(id: 13005, nameof(SignInUserIsLockedOut));
        internal static readonly EventId SignInUserIsNotAllowedEvent = new(id: 13006, nameof(SignInUserIsNotAllowed));

        internal static readonly EventId CleanUpUserRefreshTokensFailureEvent =
            new(id: 13007, nameof(CleanUpUserRefreshTokensFailure));

        internal static readonly EventId RevokeUserRefreshTokensFailureEvent =
            new(id: 13008, nameof(RevokeUserRefreshTokensFailure));

        internal static readonly EventId CreateUserRefreshTokenFailureEvent =
            new(id: 13009, nameof(CreateUserRefreshTokenFailure));
    }
}