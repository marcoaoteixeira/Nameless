using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Web.Identity.Entities;
using static Nameless.Web.Identity.Configuration.Naming;

namespace Nameless.Web.Identity.Configuration;

/// <summary>
///     Configuration for the UserToken entity.
/// </summary>
public sealed class UserTokenEntityTypeConfiguration : IEntityTypeConfiguration<UserToken> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserToken> builder) {
        builder.ToTable(UserTokens.TABLE_NAME);

        builder.HasKey(entity => new { entity.UserId, entity.LoginProvider, entity.Name })
               .HasName(UserTokens.PK);

        builder.Property(entity => entity.UserId)
               .HasColumnName(UserTokens.Fields.USER_ID);

        builder.Property(entity => entity.LoginProvider)
               .HasColumnName(UserTokens.Fields.LOGIN_PROVIDER)
               .HasMaxLength(maxLength: 512);

        builder.Property(entity => entity.Name)
               .HasColumnName(UserTokens.Fields.NAME)
               .HasMaxLength(maxLength: 512);

        builder.Property(entity => entity.Value)
               .HasColumnName(UserTokens.Fields.VALUE)
               .HasMaxLength(maxLength: 512);

        builder.HasIndex(entity => entity.UserId, UserTokens.Indexes.USER_ID)
               .IsUnique();
    }
}