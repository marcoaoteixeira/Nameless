using Microsoft.AspNetCore.Identity;

namespace Nameless.Web.Identity;

public class UserToken : IdentityUserToken<Guid> {
    public virtual User? User { get; set; }
}