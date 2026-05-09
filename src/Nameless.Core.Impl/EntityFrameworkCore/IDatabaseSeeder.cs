using Microsoft.EntityFrameworkCore;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     Defines a database seeder that populates or migrates data during
///     <see cref="Microsoft.EntityFrameworkCore.DbContext"/> initialization.
/// </summary>
public interface IDatabaseSeeder {
    /// <summary>
    ///     Asynchronously seeds the database.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="storeManagementOperation">
    ///     <see langword="true"/> if the seed is part of a store-management
    ///     (schema-creation) operation; otherwise <see langword="false"/>.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken);

    /// <summary>
    ///     Synchronously seeds the database.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="storeManagementOperation">
    ///     <see langword="true"/> if the seed is part of a store-management
    ///     (schema-creation) operation; otherwise <see langword="false"/>.
    /// </param>
    void Execute(DbContext dbContext, bool storeManagementOperation);
}