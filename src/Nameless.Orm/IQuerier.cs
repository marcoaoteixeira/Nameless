using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Orm {

    /// <summary>
    /// Defines methods/properties/events to implement a querier.
    /// </summary>
    public interface IQuerier {

        #region Methods

        /// <summary>
        /// Retrieves just one entity in the data storage that matches the specificed ID.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="id">The entity ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task" /> representing the method execution.</returns>
        Task<TEntity> FindOneAsync<TEntity>(object id, CancellationToken token = default) where TEntity : class;
        /// <summary>
        /// Retrieves just one entity based on the given expression. 
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The where-clause.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task" /> representing the method execution.</returns>
        Task<TEntity> FindOneAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken token = default) where TEntity : class;
        /// <summary>
        /// Retrieves all entities based on the specified expression.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="expression">The where-clause.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="IAsyncEnumerable{TEntity}" /> representing the collection of items to be.</returns>
        IAsyncEnumerable<TEntity> FindAllAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        /// <summary>
        /// Retrieves an <see cref="IQueryable{TEntity}" /> from the data storage.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <returns>An <see cref="IQueryable{TEntity}" />.</returns>
        IQueryable<TEntity> Query<TEntity>() where TEntity : class;

        #endregion Methods
    }
}