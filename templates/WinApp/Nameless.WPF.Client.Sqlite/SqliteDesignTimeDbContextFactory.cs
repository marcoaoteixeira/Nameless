using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nameless.WPF.Client.Sqlite;

// This is necessary to use the EF Core CLI tool
public abstract class SqliteDesignTimeDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
    where TDbContext : DbContext {

    public abstract Func<DbContextOptions<TDbContext>, TDbContext> Factory { get; }

    public TDbContext CreateDbContext(string[] args) {
        var databaseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");
        var options = new DbContextOptionsBuilder<TDbContext>()
                      .UseSqlite($"Data Source={databaseFilePath}")
                      .Options;

        return Factory(options);
    }
}