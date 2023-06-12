using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Nameless.WebApplication.Entities {

    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken> {

        #region Public Properties

        public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();

        #endregion

        #region Public Constructors

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts)
            : base(opts) { }

        #endregion

        #region Protected Override Methods

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            const string schema = "dbo";

            builder.Entity<User>(build => {
                build.ToTable(name: "Users", schema: schema);

                build.Property(_ => _.AvatarUrl);

                build.HasMany<UserRefreshToken>().WithOne().HasForeignKey(_ => _.UserId).IsRequired();
            });
            builder.Entity<Role>(build => {
                build.ToTable(name: "Roles", schema: schema);
            });
            builder.Entity<UserClaim>(build => {
                build.ToTable(name: "UserClaims", schema: schema);
            });
            builder.Entity<UserRole>(build => {
                build.ToTable(name: "UsersInRoles", schema: schema);
            });
            builder.Entity<UserLogin>(build => {
                build.ToTable(name: "UserLogins", schema: schema);
            });
            builder.Entity<RoleClaim>(build => {
                build.ToTable(name: "RoleClaims", schema: schema);
            });
            builder.Entity<UserToken>(build => {
                build.ToTable(name: "UserTokens", schema: schema);
            });
            builder.Entity<UserRefreshToken>(build => {
                build.ToTable(name: "UserRefreshTokens", schema: schema);
            });
        }

        #endregion
    }
}