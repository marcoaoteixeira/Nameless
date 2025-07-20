using Microsoft.AspNetCore.Identity;

namespace Nameless.Web.Identity.Entities;

public class UserToken : IdentityUserToken<Guid> {
    /// <summary>
    ///     Gets the user associated with the token.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual User? User { get; internal set; }
}