using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.EventSourcing.Snapshot {
    public interface ISnapshotStore {
        #region Methods

        /// <summary>
        /// Saves a snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the method execution.</returns>
        Task SaveAsync (ISnapshot snapshot, CancellationToken token = default);
        /// <summary>
        /// Retrieves a snapshot.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task{ISnapshot}"/> representing the method execution.</returns>
        Task<ISnapshot> GetAsync (Guid aggregateID, CancellationToken token = default);

        #endregion
    }
}