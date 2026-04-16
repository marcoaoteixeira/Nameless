using Microsoft.EntityFrameworkCore;

namespace Nameless.WPF.Client.Sqlite.Data;

/// <summary>
///     Represents the application <see cref="DbContext"/>.
/// </summary>
public class AppDbContext : DbContext {
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">
    ///     The database context options.
    /// </param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
}
