using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Web.Identity.Entities;
using static Nameless.Web.Identity.Configuration.Naming;

namespace Nameless.Web.Identity.Configuration;

/// <summary>
///     Configuration for the UserLogin entity.
/// </summary>
public sealed class UserLoginEntityTypeConfiguration : IEntityTypeConfiguration<UserLogin> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserLogin> builder) {
        builder.ToTable(UserLogins.TABLE_NAME);

        builder.HasKey(entity => new { entity.LoginProvider, entity.ProviderKey })
               .HasName(UserLogins.PK);

        builder.Property(entity => entity.LoginProvider)
               .HasColumnName(UserLogins.Fields.LOGIN_PROVIDER)
               .HasMaxLength(512);

        builder.Property(entity => entity.ProviderKey)
               .HasColumnName(UserLogins.Fields.PROVIDER_KEY)
               .HasMaxLength(512);

        builder.Property(entity => entity.ProviderDisplayName)
               .HasColumnName(UserLogins.Fields.PROVIDER_DISPLAY_NAME)
               .HasMaxLength(4096);

        builder.Property(entity => entity.UserId)
               .HasColumnName(UserLogins.Fields.USER_ID);

        builder.HasIndex(entity => entity.UserId, name: UserLogins.Indexes.USER_ID);
    }
}