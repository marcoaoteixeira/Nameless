using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Application;
using Nameless.WinApp.DisasterRecovery;
using Nameless.Windows;
using Nameless.Windows.EntityFramework;

namespace Nameless.WinApp.Data;

/// <summary>
///     A design time database context factory for Windows applications.
///     This is necessary to use the EF Core CLI tool
/// </summary>
public class AppDbContextDesignTimeDbContextFactory : DesignTimeDbContextFactoryWithDependencyInjection<AppDbContext> {
    /// <inheritdoc />
    public override AppDbContext CreateDbContext(string[] args) {
        var connectionString = GetConnectionString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
                      .UseSqlite(connectionString)
                      .Options;

        return new AppDbContext(options);
    }

    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration) {
        services.RegisterApplicationContext(configuration);
    }

    private string GetConnectionString() {
        var applicationContext = Services.GetRequiredService<IApplicationContext>();
        var databaseFile = applicationContext.FileSystemProvider.GetFile(
            relativePath: Path.Combine(
                applicationContext.FileSystemProvider.GetDatabaseDirectory().Path,
                SqliteConstants.DatabaseFileName
            )
        );

        return string.Format(SqliteConstants.ConnStrPattern, databaseFile.Path);
    }
}