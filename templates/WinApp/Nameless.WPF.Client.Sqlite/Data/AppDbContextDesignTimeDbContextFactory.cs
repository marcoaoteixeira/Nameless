using Microsoft.EntityFrameworkCore;
using Nameless.WPF.EntityFrameworkCore;

namespace Nameless.WPF.Client.Sqlite.Data;

// This is necessary to use the EF Core CLI tool
public class AppDbContextDesignTimeDbContextFactory : SqliteDesignTimeDbContextFactory<AppDbContext> {
    public override Func<DbContextOptions<AppDbContext>, AppDbContext> Factory => opts => new AppDbContext(opts);
}