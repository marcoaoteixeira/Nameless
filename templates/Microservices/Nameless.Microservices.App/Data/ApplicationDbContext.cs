using Microsoft.EntityFrameworkCore;
using Nameless.Microservices.App.Data.Configurations;

namespace Nameless.Microservices.App.Data;

/// <summary>
///     The main database context for the application.
/// </summary>
public class ApplicationDbContext : DbContext {
    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">
    ///     The options to be used by the context.
    /// </param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder) {
        builder.ApplyConfiguration(new ToDoItemEntityTypeConfiguration());
    }
}
