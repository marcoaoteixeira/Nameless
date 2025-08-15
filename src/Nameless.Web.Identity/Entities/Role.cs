using Microsoft.AspNetCore.Identity;

namespace Nameless.Web.Identity.Entities;

public class Role : IdentityRole<Guid> {
    /// <summary>
    ///     Gets the users-roles associated with the role.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual ICollection<UserRole> UserRoles { get; internal set; } = [];

    /// <summary>
    ///     Gets the claims associated with the role.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual ICollection<RoleClaim> RoleClaims { get; internal set; } = [];
}