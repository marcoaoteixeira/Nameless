using System.Threading;
using System.Threading.Tasks;

namespace Nameless.EventSourcing.Event {
    /// <summary>
    /// Event publisher.
    /// </summary>
    public interface IEventPublisher {

        #region Methods

        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task" /> representing the method execution.</returns>
        Task PublishAsync<TEvent> (TEvent evt, CancellationToken token = default) where TEvent : IEvent;

        #endregion Methods
    }
}