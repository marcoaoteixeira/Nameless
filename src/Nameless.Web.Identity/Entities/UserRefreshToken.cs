namespace Nameless.Web.Identity.Entities;

/// <summary>
///     Represents a refresh token entity in the identity system.
/// </summary>
public class UserRefreshToken {
    /// <summary>
    ///     Gets or sets the unique identifier for the refresh token.
    /// </summary>
    public virtual Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier for the user.
    /// </summary>
    public virtual Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the token.
    /// </summary>
    public virtual string? Token { get; set; }

    /// <summary>
    ///     Gets or sets the token expiration date and time.
    /// </summary>
    public virtual DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the refresh token was created.
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the IP address from which the refresh token was created.
    /// </summary>
    public virtual string? CreatedByIp { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the refresh token was revoked.
    /// </summary>
    public virtual DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    ///     Gets or sets the IP address from which the refresh token was revoked.
    /// </summary>
    public virtual string? RevokedByIp { get; set; }

    /// <summary>
    ///     Gets or sets the token that replaced this refresh token, if any.
    /// </summary>
    public virtual string? ReplacedByToken { get; set; }

    /// <summary>
    ///     Gets or sets the reason for revoking the refresh token, if any.
    /// </summary>
    public virtual string? RevokeReason { get; set; }

    /// <summary>
    ///     Gets the user associated with the refresh token.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual User? User { get; internal set; }
}

public static class UserRefreshTokenExtensions {
    public static bool IsRevoked(this UserRefreshToken self) {
        return self.RevokedAt is not null;
    }

    public static bool IsExpired(this UserRefreshToken self, DateTimeOffset? now = null) {
        return (now ?? DateTimeOffset.UtcNow) >= self.ExpiresAt;
    }

    public static bool IsActive(this UserRefreshToken self, DateTimeOffset? now = null) {
        return !self.IsRevoked() && !self.IsExpired(now);
    }
}