using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Barebones.Domains.Entities.Identity;

using static Nameless.Barebones.Domains.Configurations.Identity.Naming;

namespace Nameless.Barebones.Domains.Configurations.Identity;

/// <summary>
///     Configuration for the UserToken entity.
/// </summary>
public sealed class RefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<RefreshToken> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RefreshToken> builder) {
        builder.ToTable(RefreshTokens.TABLE_NAME);

        builder.Property(entity => entity.Id)
               .HasColumnName(RefreshTokens.Fields.ID)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => new { entity.Id, entity.UserId })
               .HasName(RefreshTokens.PK);

        builder.Property(entity => entity.UserId)
               .HasColumnName(RefreshTokens.Fields.USER_ID);

        builder.Property(entity => entity.Value)
               .HasColumnName(RefreshTokens.Fields.VALUE)
               .HasMaxLength(512);

        builder.Property(entity => entity.ExpiresAt)
               .HasColumnName(RefreshTokens.Fields.EXPIRES_AT)
               .IsRequired();

        builder.HasIndex(entity => entity.UserId, name: RefreshTokens.Indexes.USER_ID)
               .IsUnique();
    }
}