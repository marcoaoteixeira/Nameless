namespace Nameless.Web.Identity.Configuration;

internal static class Naming {
    internal const string CONCURRENCY_STAMP = "concurrency_stamp";

    internal static class RefreshTokens {
        internal const string TABLE_NAME = "refresh_tokens";
        internal const string PK = "pk_refresh_tokens";

        internal static class Fields {
            internal const string ID = "id";
            internal const string USER_ID = "user_id";
            internal const string TOKEN = "token";
            internal const string EXPIRES_AT = "expires_at";
            internal const string CREATED_AT = "created_at";
            internal const string CREATED_BY_IP = "created_by_ip";
            internal const string REVOKED_AT = "revoked_at";
            internal const string REVOKED_BY_IP = "revoked_by_ip";
            internal const string REPLACED_BY_TOKEN = "replaced_by_token";
            internal const string REVOKE_REASON = "revoke_reason";
        }

        internal static class Indexes {
            internal const string USER_ID = "idx_user_refresh_tokens_user_id";
            internal const string TOKEN = "idx_user_refresh_tokens_token";
        }
    }

    internal static class Roles {
        internal const string TABLE_NAME = "roles";
        internal const string PK = "pk_roles";

        internal static class Fields {
            internal const string ID = "id";
            internal const string NAME = "name";
            internal const string NORMALIZED_NAME = "normalized_name";
        }

        internal static class Indexes {
            internal const string NORMALIZED_NAME = "idx_roles_normalized_name";

            internal static class Filters {
                internal const string NORMALIZED_NAME_NOT_NULL = "normalized_name IS NOT NULL";
            }
        }

        internal static class ForeignKeys {
            internal const string USERS_ROLES = "fk_users_roles_role_id";
            internal const string ROLE_CLAIMS = "fk_role_claims_role_id";
        }
    }

    internal static class RoleClaims {
        internal const string TABLE_NAME = "role_claims";
        internal const string PK = "pk_role_claims";

        internal static class Fields {
            internal const string ID = "id";
            internal const string ROLE_ID = "role_id";
            internal const string CLAIM_TYPE = "claim_type";
            internal const string CLAIM_VALUE = "claim_value";
        }

        internal static class Indexes {
            internal const string ROLE_ID = "idx_role_claims_role_id";
        }
    }

    internal static class UserClaims {
        internal const string TABLE_NAME = "user_claims";
        internal const string PK = "pk_user_claims";

        internal static class Fields {
            internal const string ID = "id";
            internal const string CLAIM_TYPE = "claim_type";
            internal const string CLAIM_VALUE = "claim_value";
            internal const string USER_ID = "user_id";
        }

        internal static class Indexes {
            internal const string USER_ID = "idx_user_claims_user_id";
        }
    }

    internal static class Users {
        internal const string TABLE_NAME = "users";
        internal const string PK = "pk_users";

        internal static class Fields {
            internal const string ID = "id";
            internal const string FIRST_NAME = "first_name";
            internal const string LAST_NAME = "last_name";
            internal const string AVATAR_URL = "avatar_url";
            internal const string USER_NAME = "user_name";
            internal const string NORMALIZED_USER_NAME = "normalized_user_name";
            internal const string EMAIL = "email";
            internal const string NORMALIZED_EMAIL = "normalized_email";
            internal const string EMAIL_CONFIRMED = "email_confirmed";
            internal const string PASSWORD_HASH = "password_hash";
            internal const string SECURITY_STAMP = "security_stamp";
            internal const string PHONE_NUMBER = "phone_number";
            internal const string PHONE_NUMBER_CONFIRMED = "phone_number_confirmed";
            internal const string TWO_FACTOR_ENABLED = "two_factor_enabled";
            internal const string LOCKOUT_END = "lockout_end";
            internal const string LOCKOUT_ENABLED = "lockout_enabled";
            internal const string ACCESS_FAILED_COUNT = "access_failed_count";
        }

        internal static class Indexes {
            internal const string NORMALIZED_USER_NAME = "idx_users_normalized_user_name";
            internal const string NORMALIZED_EMAIL = "idx_users_normalized_email";

            internal static class Filters {
                internal const string NORMALIZED_USER_NAME_NOT_NULL = "normalized_user_name IS NOT NULL";
                internal const string NORMALIZED_EMAIL_NOT_NULL = "normalized_email IS NOT NULL";
            }
        }

        internal static class ForeignKeys {
            internal const string USERS_ROLES = "fk_users_roles_user_id";
            internal const string USER_CLAIMS = "fk_user_claims_user_id";
            internal const string USER_LOGINS = "fk_user_logins_user_id";
            internal const string USER_TOKENS = "fk_user_tokens_user_id";
            internal const string REFRESH_TOKENS = "fk_refresh_tokens_user_id";
        }
    }

    internal static class UserLogins {
        internal const string TABLE_NAME = "user_logins";
        internal const string PK = "pk_user_logins";
        internal static class Fields {
            internal const string ID = "id";
            internal const string LOGIN_PROVIDER = "login_provider";
            internal const string PROVIDER_KEY = "provider_key";
            internal const string PROVIDER_DISPLAY_NAME = "provider_display_name";
            internal const string USER_ID = "user_id";
        }
        internal static class Indexes {
            internal const string USER_ID = "idx_user_logins_user_id";
        }
    }

    internal static class UsersRoles {
        internal const string TABLE_NAME = "users_roles";
        internal const string PK = "pk_users_roles";

        internal static class Fields {
            internal const string USER_ID = "user_id";
            internal const string ROLE_ID = "role_id";
        }

        internal static class Indexes {
            internal const string USER_ID = "idx_users_roles_user_id";
            internal const string ROLE_ID = "idx_users_roles_role_id";
        }
    }

    internal static class UserTokens {
        internal const string TABLE_NAME = "user_tokens";
        internal const string PK = "pk_user_tokens";

        internal static class Fields {
            internal const string USER_ID = "user_id";
            internal const string LOGIN_PROVIDER = "login_provider";
            internal const string NAME = "name";
            internal const string VALUE = "value";
        }

        internal static class Indexes {
            internal const string USER_ID = "idx_user_tokens_user_id";
        }
    }
}
