using Microsoft.AspNetCore.Identity;

namespace Nameless.Web.Identity;

public class RoleClaim : IdentityRoleClaim<Guid> {
    public virtual Role? Role { get; set; }
}