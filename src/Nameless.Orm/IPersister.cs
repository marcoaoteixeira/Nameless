using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Orm {

    /// <summary>
    /// Declares the contract to a persister.
    /// </summary>
    public interface IPersister {

        #region Methods

        /// <summary>
        /// Saves the entities to the data storage.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entities">A collection of entities, at least one.</param>
        /// <param name="progress">The method execution progress notification.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the method execution.</returns>
        Task SaveAsync<TEntity>(TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class;
        
        /// <summary>
        /// Removes the entities from the data storage.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entities">A collection of entities, at least one.</param>
        /// <param name="progress">The method execution progress notification.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the method execution.</returns>
        Task DeleteAsync<TEntity>(TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class;

        #endregion Methods
    }
}