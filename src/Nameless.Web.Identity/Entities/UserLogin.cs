using Microsoft.AspNetCore.Identity;

namespace Nameless.Web.Identity.Entities;

public class UserLogin : IdentityUserLogin<Guid> {
    /// <summary>
    ///     Gets the user associated with the login.
    /// </summary>
    /// <remarks>
    ///     Property used for navigation purposes in Entity Framework Core.
    /// </remarks>
    public virtual User? User { get; internal set; }
}