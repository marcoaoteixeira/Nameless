﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nameless.Web.Identity.Data.Configurations;

public sealed class RoleClaimEntityTypeConfiguration : IEntityTypeConfiguration<RoleClaim> {
    public void Configure(EntityTypeBuilder<RoleClaim> builder) {
        builder.ToTable("RoleClaims");

        builder.Property(entity => entity.Id)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.ClaimType)
               .HasMaxLength(4096);

        builder.Property(entity => entity.ClaimValue)
               .HasMaxLength(4096);

        builder.Property(entity => entity.RoleId);

        builder.HasIndex(entity => entity.RoleId);
    }
}