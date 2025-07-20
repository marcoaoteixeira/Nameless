namespace Nameless.Barebones.Domains.Entities.Identity;

/// <summary>
///     Represents a refresh token entity in the identity system.
/// </summary>
public class RefreshToken {
    /// <summary>
    ///     Gets or sets the unique identifier for the refresh token.
    /// </summary>
    public virtual Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier for the user.
    /// </summary>
    public virtual Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the value of the refresh token.
    /// </summary>
    public virtual string? Value { get; set; }

    /// <summary>
    ///     Gets or sets the token expiration date and time.
    /// </summary>
    public virtual DateTime ExpiresAt { get; set; }

    /// <summary>
    ///     Gets the user associated with the refresh token.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual User? User { get; internal set; }
}
