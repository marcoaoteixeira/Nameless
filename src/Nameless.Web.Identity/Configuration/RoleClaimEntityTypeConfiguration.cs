using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Web.Identity.Entities;
using static Nameless.Web.Identity.Configuration.Naming;

namespace Nameless.Web.Identity.Configuration;

/// <summary>
///     Configuration for the RoleClaim entity.
/// </summary>
public sealed class RoleClaimEntityTypeConfiguration : IEntityTypeConfiguration<RoleClaim> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RoleClaim> builder) {
        builder.ToTable(RoleClaims.TABLE_NAME);

        builder.Property(entity => entity.Id)
               .HasColumnName(RoleClaims.Fields.ID)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => entity.Id)
               .HasName(RoleClaims.PK);

        builder.Property(entity => entity.ClaimType)
               .HasColumnName(RoleClaims.Fields.CLAIM_TYPE)
               .HasMaxLength(maxLength: 4096);

        builder.Property(entity => entity.ClaimValue)
               .HasColumnName(RoleClaims.Fields.CLAIM_VALUE)
               .HasMaxLength(maxLength: 4096);

        builder.Property(entity => entity.RoleId)
               .HasColumnName(RoleClaims.Fields.ROLE_ID);

        builder.HasIndex(entity => entity.RoleId, RoleClaims.Indexes.ROLE_ID);
    }
}