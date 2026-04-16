using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Infrastructure;
using Nameless.WPF.Client.Sqlite.Data;
using Nameless.WPF.EntityFrameworkCore;

namespace Nameless.WPF.Client.Sqlite;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterEntityFrameworkCoreForSqlite(Action<EntityFrameworkCoreRegistrationSettings> registration) {
            return self.RegisterEntityFrameworkCore<AppDbContext>(registration, (provider, builder) => {
                var applicationContext = provider.GetRequiredService<IApplicationContext>();
                var databaseFile = applicationContext.DataDirectory.GetFile(
                    Path.Combine(
                        applicationContext.GetDatabaseDirectory().Path,
                        SqliteConstants.DatabaseFileName
                    )
                );

                builder.UseSqlite(
                    string.Format(SqliteConstants.ConnStrPattern, databaseFile.Path)
                );
            });
        }
    }
}
