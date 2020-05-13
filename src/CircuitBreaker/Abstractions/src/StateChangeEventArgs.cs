using System;

namespace Nameless.CircuitBreaker {
    /// <summary>
    /// State change event arguments.
    /// </summary>
	public sealed class StateChangeEventArgs : EventArgs {

        #region Public Properties

        /// <summary>
        /// Gets the current state of the circuit breaker.
        /// </summary>
        public CircuitBreakerState State { get; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initalizes a new instance of <see cref="StateChangeEventArgs"/>.
        /// </summary>
        /// <param name="state">The current state of the circuit breaker.</param>
        public StateChangeEventArgs (CircuitBreakerState state) {
            State = state;
        }

        #endregion
    }
}
