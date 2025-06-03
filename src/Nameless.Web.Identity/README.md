# Nameless Web Identity Library

This library was written to be used with ASP.Net Core projects.

## Identity Db Context Sample

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nameless.Web.Identity.Data.Configurations;
using Nameless.Web.Identity;

namespace My.Web.App;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken> {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.ApplyConfiguration(new RoleClaimEntityTypeConfiguration());
        builder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserClaimEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserLoginEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserTokenEntityTypeConfiguration());
    }
}
```