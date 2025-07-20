using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Barebones.Domains.Entities.Identity;

using static Nameless.Barebones.Domains.Configurations.Identity.Naming;

namespace Nameless.Barebones.Domains.Configurations.Identity;

/// <summary>
///     Configuration for the UserRole entity.
/// </summary>
public sealed class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserRole> builder) {
        builder.ToTable(UsersRoles.TABLE_NAME);

        builder.HasKey(entity => new { entity.UserId, entity.RoleId })
               .HasName(UsersRoles.PK);

        builder.Property(entity => entity.UserId)
               .HasColumnName(UsersRoles.Fields.USER_ID);

        builder.Property(entity => entity.RoleId)
               .HasColumnName(UsersRoles.Fields.ROLE_ID);

        builder.HasIndex(entity => entity.RoleId, name: UsersRoles.Indexes.ROLE_ID);

        builder.HasIndex(entity => entity.UserId, name: UsersRoles.Indexes.USER_ID);
    }
}