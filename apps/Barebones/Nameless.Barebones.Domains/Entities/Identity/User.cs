using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class User : IdentityUser<Guid> {
    /// <summary>
    ///     Gets or sets the first name of the user.
    /// </summary>
    [MaxLength(256)]
    public string? FirstName { get; set; }

    /// <summary>
    ///     Get or sets the last name of the user.
    /// </summary>
    [MaxLength(256)]
    public string? LastName { get; set; }

    /// <summary>
    ///     Gets or sets the URL to the user avatar.
    /// </summary>
    [MaxLength(1024)]
    public string? AvatarUrl { get; set; }

    /// <summary>
    ///     Gets or sets the claims associated with the user.
    /// </summary>
    public virtual ICollection<UserClaim> Claims { get; internal set; } = [];

    /// <summary>
    ///     Gets or sets the logins associated with the user.
    /// </summary>
    public virtual ICollection<UserLogin> Logins { get; internal set; } = [];

    /// <summary>
    ///     Gets or sets the tokens associated with the user.
    /// </summary>
    public virtual ICollection<UserToken> Tokens { get; internal set; } = [];

    /// <summary>
    ///     Gets or sets the roles associated with the user.
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; internal set; } = [];

    /// <summary>
    ///     Gets or sets the refresh tokens associated with the user.
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; internal set; } = [];
}