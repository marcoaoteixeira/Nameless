using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Barebones.Domains.Entities.Identity;

using static Nameless.Barebones.Domains.Configurations.Identity.Naming;

namespace Nameless.Barebones.Domains.Configurations.Identity;

/// <summary>
///     Configuration for the UserClaim entity.
/// </summary>
public sealed class UserClaimEntityTypeConfiguration : IEntityTypeConfiguration<UserClaim> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserClaim> builder) {
        builder.ToTable(UserClaims.TABLE_NAME);

        builder.Property(entity => entity.Id)
               .HasColumnName(UserClaims.Fields.ID)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => entity.Id)
               .HasName(UserClaims.PK);

        builder.Property(entity => entity.ClaimType)
               .HasColumnName(UserClaims.Fields.CLAIM_TYPE)
               .HasMaxLength(4096);

        builder.Property(entity => entity.ClaimValue)
               .HasColumnName(UserClaims.Fields.CLAIM_VALUE)
               .HasMaxLength(4096);

        builder.Property(entity => entity.UserId)
               .HasColumnName(UserClaims.Fields.USER_ID);

        builder.HasIndex(entity => entity.UserId, name: UserClaims.Indexes.USER_ID);
    }
}