using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nameless.EntityFrameworkCore.Entities;
using EntityState = Nameless.EntityFrameworkCore.Entities.EntityState;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     <see cref="DbSet{TEntity}"/> extension methods.
/// </summary>
public static class DbSetExtensions {
    /// <typeparam name="TEntity">
    ///     Type of the entity.
    /// </typeparam>
    /// <param name="self">
    ///     The current instance of <see cref="DbSet{TEntity}"/>.
    /// </param>
    extension<TEntity>(DbSet<TEntity> self)
        where TEntity : EntityBase {
        /// <summary>
        ///     Soft delete the entity instance.
        /// </summary>
        /// <param name="entity">
        ///     The entity instance.
        /// </param>
        /// <param name="timeProvider">
        ///     The time provider.
        /// </param>
        /// <returns>
        ///     The entity entry.
        /// </returns>
        public EntityEntry<TEntity> SoftRemove(TEntity entity, TimeProvider? timeProvider = null) {
            var now = (timeProvider ?? TimeProvider.System).GetUtcNow();

            return self.MarkDeleted(entity, now);
        }

        /// <summary>
        ///     Soft delete a collection of entity instance.
        /// </summary>
        /// <param name="entities">
        ///     A collection of entity instance.
        /// </param>
        /// <param name="timeProvider">
        ///     The time provider.
        /// </param>
        public void SoftRemoveRange(TEntity[] entities, TimeProvider? timeProvider = null) {
            var now = (timeProvider ?? TimeProvider.System).GetUtcNow();

            foreach (var entity in entities) {
                _ = self.MarkDeleted(entity, now);
            }
        }

        private EntityEntry<TEntity> MarkDeleted(TEntity entity, DateTimeOffset now) {
            // no-op for state if already tracked;
            // doesn't cascade as Modified
            var entry = self.Attach(entity);

            if (entity.EntityState == EntityState.Deleted) {
                return entry; // already soft-deleted, avoid a pointless UPDATE
            }

            if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Added) {
                // Never persisted: removing it means cancelling the pending insert,
                // same as DbSet.Remove does for Added entities.
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            entity.EntityState = EntityState.Deleted;
            entity.ModificationDate = now;

            entry.Property(obj => obj.EntityState).IsModified = true;
            entry.Property(obj => obj.ModificationDate).IsModified = true;

            return entry;
        }
    }
}