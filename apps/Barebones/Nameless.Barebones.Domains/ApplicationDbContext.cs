using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nameless.Barebones.Domains.Configurations.Identity;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Domains;

/// <summary>
///     The database context for the application, inheriting from IdentityDbContext.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken> {
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">
    ///     The db context options.
    /// </param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder) {
        builder.ApplyConfiguration(new RoleClaimEntityTypeConfiguration());
        builder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserClaimEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserLoginEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserTokenEntityTypeConfiguration());
        builder.ApplyConfiguration(new RefreshTokenEntityTypeConfiguration());
    }
}