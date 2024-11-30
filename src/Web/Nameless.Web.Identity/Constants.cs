namespace Nameless.Web.Identity;

internal static class Constants {
    internal static class DbContext {
        internal static class Fields {
            internal const string CONCURRENCY_STAMP_COLUMN_NAME = "ConcurrencyStamp";
        }
    }

    internal static class Endpoints {
        internal const string SIGN_IN = "/accounts/sign-in";
        internal const string TWO_FACTOR_AUTH = "/accounts/two-factor-auth";
        internal const string CONFIRM_EMAIL = "/accounts/confirm-email";
    }

    internal static class Messages {
        internal static class SignIn {
            internal const string USER_LOCKED_OUT_MESSAGE = "User account is locked out.";
            internal const string USER_LOCKED_OUT_TITLE = "User Locked Out";
            internal const string INVALID_SIGNIN_ATTEMPT_MESSAGE = "Invalid sign in attempt.";
            internal const string INVALID_SIGNIN_ATTEMPT_TITLE = "Invalid Sign In";
        }

        internal static class TwoFactorAuth {
            internal const string UNABLE_LOAD_USER_MESSAGE = "Unable to load two-factor authentication user.";
            internal const string UNABLE_LOAD_USER_TITLE = "Missing User.";
            internal const string USER_LOCKED_OUT_MESSAGE = "User with ID '{0}' account is locked out.";
            internal const string USER_LOCKED_OUT_TITLE = "User Locked Out";
            internal const string INVALID_CODE_MESSAGE = "Invalid authenticator code entered for user with ID '{0}'.";
            internal const string INVALID_CODE_TITLE = "Invalid Authenticator Code";
        }

        internal static class ConfirmEmail {
            internal const string UNABLE_LOAD_USER_MESSAGE = "Error loading user with ID '{0}'.";
            internal const string UNABLE_LOAD_USER_TITLE = "Error Loading User";
            internal const string CONFIRM_EMAIL_SUCCEEDED = "Thank you for confirming your email.";
            internal const string CONFIRM_EMAIL_FAILED_MESSAGE = "Error confirming your email.";
            internal const string CONFIRM_EMAIL_FAILED_TITLE = "Confirm E-mail";

        }
    }
}