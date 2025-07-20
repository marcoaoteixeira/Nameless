using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class UserRole : IdentityUserRole<Guid> {
    /// <summary>
    ///     Gets the user associated with the entity.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual User? User { get; internal set; }

    /// <summary>
    ///     Gets the role associated with the entity.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual Role? Role { get; internal set; }
}