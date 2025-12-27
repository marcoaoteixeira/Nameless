using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Web.Identity.Entities;
using static Nameless.Web.Identity.Configuration.Naming;

namespace Nameless.Web.Identity.Configuration;

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

        builder.HasIndex(entity => entity.RoleId, UsersRoles.Indexes.ROLE_ID);

        builder.HasIndex(entity => entity.UserId, UsersRoles.Indexes.USER_ID);
    }
}