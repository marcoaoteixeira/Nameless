using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Web.Identity.Entities;
using static Nameless.Web.Identity.Configuration.Naming;

namespace Nameless.Web.Identity.Configuration;

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
               .HasMaxLength(maxLength: 4096);

        builder.Property(entity => entity.ClaimValue)
               .HasColumnName(UserClaims.Fields.CLAIM_VALUE)
               .HasMaxLength(maxLength: 4096);

        builder.Property(entity => entity.UserId)
               .HasColumnName(UserClaims.Fields.USER_ID);

        builder.HasIndex(entity => entity.UserId, UserClaims.Indexes.USER_ID);
    }
}