using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Web.Identity.Entities;
using static Nameless.Web.Identity.Configuration.Naming;

namespace Nameless.Web.Identity.Configuration;

/// <summary>
///     Configuration for the Role entity.
/// </summary>
public sealed class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Role> builder) {
        builder.ToTable(Roles.TABLE_NAME);

        builder.Property(entity => entity.Id)
               .HasColumnName(Roles.Fields.ID)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => entity.Id)
               .HasName(Roles.PK);

        builder.Property(entity => entity.Name)
               .HasColumnName(Roles.Fields.NAME)
               .HasMaxLength(maxLength: 256);

        builder.Property(entity => entity.NormalizedName)
               .HasColumnName(Roles.Fields.NORMALIZED_NAME)
               .HasMaxLength(maxLength: 256);

        builder.Property<string>(CONCURRENCY_STAMP)
               .HasMaxLength(int.MaxValue)
               .IsConcurrencyToken();

        builder.HasIndex(entity => entity.NormalizedName, Roles.Indexes.NORMALIZED_NAME)
               .IsUnique()
               .HasFilter(Roles.Indexes.Filters.NORMALIZED_NAME_NOT_NULL);

        // Each Role can have many entries in the UserRole join table
        builder.HasMany(entity => entity.UserRoles)
               .WithOne(entity => entity.Role)
               .HasForeignKey(userRole => userRole.RoleId)
               .HasConstraintName(Roles.ForeignKeys.USERS_ROLES)
               .IsRequired();

        // Each Role can have many associated RoleClaims
        builder.HasMany(entity => entity.RoleClaims)
               .WithOne(entity => entity.Role)
               .HasForeignKey(roleClaim => roleClaim.RoleId)
               .HasConstraintName(Roles.ForeignKeys.ROLE_CLAIMS)
               .IsRequired();
    }
}