using System;
using System.Threading;
using System.Threading.Tasks;
using Nameless.EventSourcing.Domain;

namespace Nameless.EventSourcing.Repository {
    /// <summary>
    /// Aggregate repository
    /// </summary>
    public interface IRepository {

        #region Public Methods

        /// <summary>
        /// Saves an aggregate.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the method execution.</returns>
        Task SaveAsync (AggregateRoot aggregate, CancellationToken token = default);

        /// <summary>
        /// Retrieves an aggregate.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task{TAggregate}"/> representing the method execution.</returns>
        Task<TAggregate> GetAsync<TAggregate> (Guid aggregateID, CancellationToken token = default) where TAggregate : AggregateRoot;

        #endregion Public Methods
    }
}