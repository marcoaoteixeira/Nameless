using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Web.Identity.Entities;
using static Nameless.Web.Identity.Configuration.Naming;

namespace Nameless.Web.Identity.Configuration;

/// <summary>
///     Configuration for the User entity.
/// </summary>
public sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder) {
        builder.ToTable(Users.TABLE_NAME);

        // Key
        builder.Property(entity => entity.Id)
               .HasColumnName(Users.Fields.ID)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => entity.Id)
               .HasName(Users.PK);

        // Fields
        builder.Property(entity => entity.FirstName)
               .HasColumnName(Users.Fields.FIRST_NAME)
               .HasMaxLength(256);

        builder.Property(entity => entity.LastName)
               .HasColumnName(Users.Fields.LAST_NAME)
               .HasMaxLength(256);

        builder.Property(entity => entity.UserName)
               .HasColumnName(Users.Fields.USER_NAME)
               .HasMaxLength(256);

        builder.Property(entity => entity.NormalizedUserName)
               .HasColumnName(Users.Fields.NORMALIZED_USER_NAME)
               .HasMaxLength(256);

        builder.Property(entity => entity.Email)
               .HasColumnName(Users.Fields.EMAIL)
               .HasMaxLength(256);

        builder.Property(entity => entity.NormalizedEmail)
               .HasColumnName(Users.Fields.NORMALIZED_EMAIL)
               .HasMaxLength(256);

        builder.Property(entity => entity.EmailConfirmed)
               .HasColumnName(Users.Fields.EMAIL_CONFIRMED);

        builder.Property(entity => entity.PhoneNumber)
               .HasColumnName(Users.Fields.PHONE_NUMBER)
               .HasMaxLength(64);

        builder.Property(entity => entity.PhoneNumberConfirmed)
               .HasColumnName(Users.Fields.PHONE_NUMBER_CONFIRMED);

        builder.Property(entity => entity.AvatarUrl)
               .HasColumnName(Users.Fields.AVATAR_URL)
               .HasMaxLength(1024);

        builder.Property(entity => entity.LockoutEnabled)
               .HasColumnName(Users.Fields.LOCKOUT_ENABLED);

        builder.Property(entity => entity.LockoutEnd)
               .HasColumnName(Users.Fields.LOCKOUT_END)
               .IsRequired(false);

        builder.Property(entity => entity.PasswordHash)
               .HasColumnName(Users.Fields.PASSWORD_HASH)
               .HasMaxLength(4096);

        builder.Property(entity => entity.SecurityStamp)
               .HasColumnName(Users.Fields.SECURITY_STAMP)
               .HasMaxLength(4096);

        builder.Property(entity => entity.TwoFactorEnabled)
               .HasColumnName(Users.Fields.TWO_FACTOR_ENABLED);

        builder.Property(entity => entity.AccessFailedCount)
               .HasColumnName(Users.Fields.ACCESS_FAILED_COUNT);

        builder.Property<string>(CONCURRENCY_STAMP)
               .IsConcurrencyToken()
               .HasMaxLength(int.MaxValue);

        // Indexes
        builder.HasIndex(entity => entity.NormalizedUserName, name: Users.Indexes.NORMALIZED_USER_NAME)
               .IsUnique()
               .HasFilter(Users.Indexes.Filters.NORMALIZED_USER_NAME_NOT_NULL);

        builder.HasIndex(entity => entity.NormalizedEmail, name: Users.Indexes.NORMALIZED_EMAIL)
               .IsUnique()
               .HasFilter(Users.Indexes.Filters.NORMALIZED_EMAIL_NOT_NULL);

        // Each User can have many UserClaims
        builder.HasMany(entity => entity.Claims)
               .WithOne(entity => entity.User)
               .HasForeignKey(userClaim => userClaim.UserId)
               .HasConstraintName(Users.ForeignKeys.USER_CLAIMS)
               .IsRequired();

        // Each User can have many UserLogins
        builder.HasMany(entity => entity.Logins)
               .WithOne(entity => entity.User)
               .HasForeignKey(userLogin => userLogin.UserId)
               .HasConstraintName(Users.ForeignKeys.USER_LOGINS)
               .IsRequired();

        // Each User can have many UserTokens
        builder.HasMany(entity => entity.Tokens)
               .WithOne(entity => entity.User)
               .HasForeignKey(userToken => userToken.UserId)
               .HasConstraintName(Users.ForeignKeys.USER_TOKENS)
               .IsRequired();

        // Each User can have many entries in the UserRole join table
        builder.HasMany(entity => entity.UserRoles)
               .WithOne(entity => entity.User)
               .HasForeignKey(userRole => userRole.UserId)
               .HasConstraintName(Users.ForeignKeys.USERS_ROLES)
               .IsRequired();

        // Each User can have many RefreshTokens
        builder.HasMany(entity => entity.RefreshTokens)
               .WithOne(entity => entity.User)
               .HasForeignKey(refreshToken => refreshToken.UserId)
               .HasConstraintName(Users.ForeignKeys.REFRESH_TOKENS)
               .IsRequired();
    }
}