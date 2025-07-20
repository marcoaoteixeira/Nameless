using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Nameless.Barebones.Domains;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace Nameless.Barebones.Api.Configs;

public static class DbContextConfig {
    public const string DATABASE_NAME = "barebones-db";

    public static WebApplicationBuilder RegisterDbContext(this WebApplicationBuilder self) {
        self.Services.AddDbContext<ApplicationDbContext>(options => {
            // "barebones-db" is the name of the connection configured by Aspire.
            options
                .UseNpgsql(self.Configuration.GetConnectionString(DATABASE_NAME), opts => {
                    opts.MigrationsHistoryTable("__ef_migration_history", "public");
                })
                .ReplaceService<IHistoryRepository, CustomNpgsqlHistoryRepository>();
        });

        return self;
    }

    public static WebApplication UseDbContext(this WebApplication self) {
        self.Lifetime.ApplicationStarted.Register(state => {
            if (state is not WebApplication app) {
                throw new InvalidOperationException("Application instance is null.");
            }

            using var activitySource = new ActivitySource(nameof(DbContextConfig));
            using var activity = activitySource.StartActivity(name: "Database Migration");

            //var activity = app.Services
            //                   .GetRequiredService<IActivitySourceManager>()
            //                   .GetActivitySource(nameof(DbContextConfig))
            //                   .StartActivity();
            var logger = app.Services.GetLogger<ApplicationDbContext>();

            try {
                logger.MigrationStarted(DATABASE_NAME);

                using var scope = app.Services.CreateScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Apply any pending migrations.
                dbContext.Database.Migrate();
            }
            catch (Exception ex) {
                activity?.AddException(ex);

                throw;
            }
            finally { logger.MigrationFinished(DATABASE_NAME); }
        }, self);

        return self;
    }
}

#pragma warning disable EF1001
internal class CustomNpgsqlHistoryRepository : NpgsqlHistoryRepository {
    public CustomNpgsqlHistoryRepository(HistoryRepositoryDependencies dependencies)
        : base(dependencies) { }

    protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history) {
        base.ConfigureTable(history);

        history.Property(prop => prop.MigrationId).HasColumnName("migration_id");
        history.Property(prop => prop.ProductVersion).HasColumnName("product_version");
    }
}
#pragma warning restore EF1001
