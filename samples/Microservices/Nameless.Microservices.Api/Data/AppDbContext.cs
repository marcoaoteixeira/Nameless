using Microsoft.EntityFrameworkCore;
using Nameless.Microservices.Api.Data.Configs;
using Nameless.Microservices.Api.Domains.Chores.Entities;

namespace Nameless.Microservices.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts) {
    public DbSet<Chore> Chores => Set<Chore>();

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.ApplyConfiguration(new ChoreEntityTypeConfiguration());
    }
}
