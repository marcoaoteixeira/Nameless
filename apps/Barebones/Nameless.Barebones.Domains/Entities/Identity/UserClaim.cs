using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class UserClaim : IdentityUserClaim<Guid> {
    /// <summary>
    ///     Gets the user associated with the claim.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual User? User { get; internal set; }
}