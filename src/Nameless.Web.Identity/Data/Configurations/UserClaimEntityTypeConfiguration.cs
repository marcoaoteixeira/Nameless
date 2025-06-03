using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nameless.Web.Identity.Data.Configurations;

public sealed class UserClaimEntityTypeConfiguration : IEntityTypeConfiguration<UserClaim> {
    public void Configure(EntityTypeBuilder<UserClaim> builder) {
        builder.ToTable("UserClaims");

        builder.Property(entity => entity.Id)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.ClaimType)
               .HasMaxLength(4096);

        builder.Property(entity => entity.ClaimValue)
               .HasMaxLength(4096);

        builder.Property(entity => entity.UserId);

        builder.HasIndex(entity => entity.UserId);
    }
}