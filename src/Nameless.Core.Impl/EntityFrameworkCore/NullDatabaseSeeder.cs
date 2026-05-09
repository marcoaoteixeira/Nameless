using Microsoft.EntityFrameworkCore;
using Nameless.Registration;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     A no-op implementation of <see cref="IDatabaseSeeder"/> used when no
///     seeder is registered.
/// </summary>
[IgnoreAssemblyScan]
public sealed class NullDatabaseSeeder : IDatabaseSeeder {
    /// <summary>
    ///     Gets the singleton instance.
    /// </summary>
    public static IDatabaseSeeder Instance { get; } = new NullDatabaseSeeder();

    static NullDatabaseSeeder() { }

    private NullDatabaseSeeder() { }

    /// <inheritdoc />
    public Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Execute(DbContext dbContext, bool storeManagementOperation) {
        /* do nothing */
    }
}