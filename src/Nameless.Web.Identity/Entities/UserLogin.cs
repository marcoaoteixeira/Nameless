using Microsoft.AspNetCore.Identity;

namespace Nameless.Web.Identity;

public class UserLogin : IdentityUserLogin<Guid> {
    public virtual User? User { get; set; }
}