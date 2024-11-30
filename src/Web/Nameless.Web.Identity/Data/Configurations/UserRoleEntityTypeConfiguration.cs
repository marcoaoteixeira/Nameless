using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nameless.Web.Identity.Data.Configurations;

public sealed class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole> {
    public void Configure(EntityTypeBuilder<UserRole> builder) {
        builder.ToTable("UserRoles");

        builder.HasKey(entity => new { entity.UserId, entity.RoleId });

        builder.Property(entity => entity.UserId);

        builder.Property(entity => entity.RoleId);

        builder.HasIndex(entity => entity.RoleId);

        builder.HasIndex(entity => entity.UserId);
    }
}