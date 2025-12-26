using Microsoft.EntityFrameworkCore;
using Nameless.Microservices.App.Data;

namespace Nameless.Microservices.App.Configs;

/// <summary>
///     Configuration for the database context.
/// </summary>
public static class DbContextConfig {
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/> instance.
    /// </param>
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures the database context for the application.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureDbContext() {
            self.Services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlite(
                    connectionString: self.Configuration.GetConnectionString("Sqlite")
                );
            });

            return self;
        }
    }

    extension(WebApplication self) {
        public WebApplication UseDbContext() {
            if (!bool.TryParse(self.Configuration["EFCore:UseMigration"], out var useMigration) || !useMigration) {
                return self;
            }

            using var scope = self.Services.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbContext.Database.IsRelational()) {
                dbContext.Database.Migrate();
            }

            return self;
        }
    }
}
