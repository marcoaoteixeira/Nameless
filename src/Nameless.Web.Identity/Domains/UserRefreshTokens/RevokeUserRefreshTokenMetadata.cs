namespace Nameless.Web.Identity.Domains.UserRefreshTokens;

public readonly record struct RevokeUserRefreshTokenMetadata(
    DateTimeOffset? ExpiresAt = null,
    string? RevokedByIp = null,
    string? RevokeReason = null,
    string? ReplacedByToken = null
);