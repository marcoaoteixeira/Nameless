using Microsoft.EntityFrameworkCore;
using Nameless.Microservices.App.Data;

namespace Nameless.Microservices.App.Configs;

/// <summary>
///     Configuration for the database context.
/// </summary>
public static class DbContextConfig {
    /// <summary>
    ///     Configures the database context for the application.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/> instance.
    /// </param>
    /// <returns>
    ///     The current <see cref="WebApplicationBuilder"/> so other actions
    ///     can be chained.
    /// </returns>
    public static WebApplicationBuilder ConfigureDbContext(this WebApplicationBuilder self) {
        self.Services.AddDbContext<ApplicationDbContext>(options => {
            options.UseSqlite("Data Source=App_Data/application.db;");
        });

        return self;
    }

    public static WebApplication UseDbContext(this WebApplication self) {
        using var scope = self.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (dbContext.Database.IsRelational()) {
            dbContext.Database.Migrate();
        }

        return self;
    }
}
