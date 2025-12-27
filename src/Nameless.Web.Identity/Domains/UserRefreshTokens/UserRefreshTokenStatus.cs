namespace Nameless.Web.Identity.Domains.UserRefreshTokens;

[Flags]
public enum UserRefreshTokenStatus {
    Active = 1,

    Revoked = 2,

    Expired = 4,

    Inactive = Revoked | Expired,

    Unknown = 8
}