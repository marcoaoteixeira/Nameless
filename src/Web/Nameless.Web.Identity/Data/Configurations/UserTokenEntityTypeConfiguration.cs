﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nameless.Web.Identity.Data.Configurations;

public sealed class UserTokenEntityTypeConfiguration : IEntityTypeConfiguration<UserToken> {
    public void Configure(EntityTypeBuilder<UserToken> builder) {
        builder.ToTable("UserTokens");

        builder.HasKey(entity => new {
            entity.UserId,
            entity.LoginProvider,
            entity.Name
        });

        builder.Property(entity => entity.UserId);

        builder.Property(entity => entity.LoginProvider)
               .HasMaxLength(512);

        builder.Property(entity => entity.Name)
               .HasMaxLength(512);

        builder.Property(entity => entity.Value)
               .HasMaxLength(512);

        builder.HasIndex(entity => entity.UserId);
    }
}