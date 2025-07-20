using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Web.Identity.Entities;
using static Nameless.Web.Identity.Configuration.Naming;

namespace Nameless.Web.Identity.Configuration;

/// <summary>
///     Configuration for the UserToken entity.
/// </summary>
public sealed class UserRefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<UserRefreshToken> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder) {
        builder.ToTable(RefreshTokens.TABLE_NAME);

        builder.Property(entity => entity.Id)
               .HasColumnName(RefreshTokens.Fields.ID)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => new { entity.Id, entity.UserId })
               .HasName(RefreshTokens.PK);

        builder.Property(entity => entity.UserId)
               .HasColumnName(RefreshTokens.Fields.USER_ID);

        builder.Property(entity => entity.Token)
               .HasColumnName(RefreshTokens.Fields.TOKEN)
               .HasMaxLength(512);

        builder.Property(entity => entity.ExpiresAt)
               .HasColumnName(RefreshTokens.Fields.EXPIRES_AT)
               .IsRequired();

        builder.Property(entity => entity.CreatedAt)
               .HasColumnName(RefreshTokens.Fields.CREATED_AT);

        builder.Property(entity => entity.CreatedByIp)
               .HasColumnName(RefreshTokens.Fields.CREATED_BY_IP);

        builder.Property(entity => entity.RevokedAt)
               .HasColumnName(RefreshTokens.Fields.REVOKED_AT);

        builder.Property(entity => entity.RevokedByIp)
               .HasColumnName(RefreshTokens.Fields.REVOKED_BY_IP);

        builder.Property(entity => entity.RevokeReason)
               .HasColumnName(RefreshTokens.Fields.REVOKE_REASON);

        builder.Property(entity => entity.ReplacedByToken)
               .HasColumnName(RefreshTokens.Fields.REPLACED_BY_TOKEN);

        builder.HasIndex(entity => entity.UserId, name: RefreshTokens.Indexes.USER_ID)
               .IsUnique();

        builder.HasIndex(entity => entity.Token, name: RefreshTokens.Indexes.TOKEN)
               .IsUnique();
    }
}