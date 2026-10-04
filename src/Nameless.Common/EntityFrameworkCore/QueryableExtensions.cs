using Microsoft.EntityFrameworkCore;
using Nameless.EntityFrameworkCore.Entities;
using EntityState = Nameless.EntityFrameworkCore.Entities.EntityState;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     <see cref="IQueryable{T}"/> extension methods.
/// </summary>
public static class QueryableExtensions {
    /// <typeparam name="TEntity">
    ///     Type of the entity.
    /// </typeparam>
    /// <param name="self">
    ///     The current instance of <see cref="IQueryable{T}"/>.
    /// </param>
    extension<TEntity>(IQueryable<TEntity> self)
        where TEntity : EntityBase {
        /// <summary>
        ///     Soft delete all database rows for the entity instances which
        ///     match the LINQ query from the database.
        /// </summary>
        /// <param name="timeProvider">
        ///     The time provider.
        /// </param>
        /// <remarks>
        ///     This operation executes immediately against the database,
        ///     rather than being deferred until
        ///     <c>Microsoft.EntityFrameworkCore.DbContext.SaveChanges</c>
        ///     is called. It also does not interact with the EF change
        ///     tracker in any way: entity instances which happen to be
        ///     tracked when this operation is invoked aren't taken into
        ///     account, and aren't updated to reflect the changes.
        /// </remarks>
        /// <returns>
        ///     The total number of rows updated in the database.
        /// </returns>
        public int ExecuteSoftDelete(TimeProvider? timeProvider = null) {
            var now = (timeProvider ?? TimeProvider.System).GetUtcNow();

            return self.Where(entity => entity.EntityState != EntityState.Deleted)
                       .ExecuteUpdate(update => update.SetProperty(entity => entity.EntityState, EntityState.Deleted)
                                                      .SetProperty(entity => entity.ModificationDate, now)
                        );
        }

        /// <summary>
        ///     Soft delete all database rows for the entity instances which
        ///     match the LINQ query from the database.
        /// </summary>
        /// <param name="timeProvider">
        ///     The time provider.
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <remarks>
        ///     This operation executes immediately against the database,
        ///     rather than being deferred until
        ///     <c>Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync</c>
        ///     is called. It also does not interact with the EF change
        ///     tracker in any way: entity instances which happen to be
        ///     tracked when this operation is invoked aren't taken into
        ///     account, and aren't updated to reflect the changes.
        /// </remarks>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> representing the asynchronous
        ///     method execution where the result is the total number of
        ///     rows updated in the database.
        /// </returns>
        public Task<int> ExecuteSoftDeleteAsync(CancellationToken cancellationToken = default, TimeProvider? timeProvider = null) {
            var now = (timeProvider ?? TimeProvider.System).GetUtcNow();

            return self.Where(entity => entity.EntityState != EntityState.Deleted)
                       .ExecuteUpdateAsync(update => update.SetProperty(entity => entity.EntityState, EntityState.Deleted)
                                                           .SetProperty(entity => entity.ModificationDate, now),
                           cancellationToken
                       );
        }
    }
}