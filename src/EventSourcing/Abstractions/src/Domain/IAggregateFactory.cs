using System.Threading;
using System.Threading.Tasks;

namespace Nameless.EventSourcing.Domain {
    /// <summary>
    /// Aggregate factory.
    /// </summary>
    public interface IAggregateFactory {

        #region Methods

        /// <summary>
        /// Creates an aggregate.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <param name="args">Arguments passed to the object creation cycle.</param>
        /// <returns>A <see cref="Task{TAggregate}"/> representing the method execution.</returns>
        Task<TAggregate> CreateAsync<TAggregate> (CancellationToken token = default, params object[] args) where TAggregate : AggregateRoot;

        #endregion
    }
}