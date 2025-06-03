using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nameless.Web.Identity.Data.Configurations;

public sealed class UserLoginEntityTypeConfiguration : IEntityTypeConfiguration<UserLogin> {
    public void Configure(EntityTypeBuilder<UserLogin> builder) {
        builder.ToTable("UserLogins");

        builder.HasKey(entity => new { entity.LoginProvider, entity.ProviderKey });

        builder.Property(entity => entity.LoginProvider)
               .HasMaxLength(512);

        builder.Property(entity => entity.ProviderKey)
               .HasMaxLength(512);

        builder.Property(entity => entity.ProviderDisplayName)
               .HasMaxLength(4096);

        builder.Property(entity => entity.UserId);

        builder.HasIndex(entity => entity.UserId);
    }
}