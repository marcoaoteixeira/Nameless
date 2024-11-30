using Microsoft.AspNetCore.Identity;

namespace Nameless.Web.Identity;

public class UserClaim : IdentityUserClaim<Guid> {
    public virtual User? User { get; set; }
}