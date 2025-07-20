using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class RoleClaim : IdentityRoleClaim<Guid> {
    /// <summary>
    ///     Gets the role associated with the claim.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual Role? Role { get; internal set; }
}