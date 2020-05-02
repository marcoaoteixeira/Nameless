using System;
using System.Threading;
using System.Threading.Tasks;
using Nameless.EventSourcing.Domain;

namespace Nameless.EventSourcing.Repository {
    /// <summary>
    /// Session state.
    /// </summary>
    public interface ISession {
        #region Public Methods

        /// <summary>
        /// Attaches an aggregate to the current session.
        /// </summary>
        /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
        /// <param name="aggregate">The instance of the aggregate.</param>
        void Attach<TAggregate> (TAggregate aggregate) where TAggregate : AggregateRoot;

        /// <summary>
        /// Detaches an aggregate from the current session.
        /// </summary>
        /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
        /// <param name="aggregate">The instance of the aggregate.</param>
        void Detach<TAggregate> (TAggregate aggregate) where TAggregate : AggregateRoot;

        /// <summary>
        /// Detaches all aggregates from the current session.
        /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
        /// </summary>
        void DetachAll<TAggregate> () where TAggregate : AggregateRoot;

        /// <summary>
        /// Retrieves an aggregate from the current session.
        /// </summary>
        /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task{TAggregate}"> representing the method execution.</returns>
        Task<TAggregate> GetAsync<TAggregate> (Guid aggregateID, CancellationToken token = default) where TAggregate : AggregateRoot;

        /// <summary>
        /// Committs all changes.
        /// </summary>
        /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task"> representing the method execution.</returns>
        Task CommittAsync<TAggregate> (CancellationToken token = default) where TAggregate : AggregateRoot;

        #endregion
    }
}