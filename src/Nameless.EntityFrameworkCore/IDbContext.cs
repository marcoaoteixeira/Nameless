using Microsoft.EntityFrameworkCore;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     Interface to intermediate the calls for <see cref="DbContext"/> methods.
/// </summary>
public interface IDbContext {
    /// <summary>
    ///     Creates a <see cref="DbSet{TEntity}"/> that can be used to query
    ///     and save instances of <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity.
    /// </typeparam>
    /// <returns>
    ///     A set for the given entity type.
    /// </returns>
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    /// <summary>
    ///     Creates a <see cref="DbSet{TEntity}"/> for a shared-type entity
    ///     type that can be used to query and save instances of
    ///     <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity.
    /// </typeparam>
    /// <param name="name">
    ///     The name for the shared-type entity type to use.
    /// </param>
    /// <returns>
    ///     A set for the given entity type.
    /// </returns>
    DbSet<TEntity> Set<TEntity>(string name)
        where TEntity : class;

    /// <summary>
    ///     Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous save operation. The task
    ///     result contains the number of state entries written to the
    ///     database.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}