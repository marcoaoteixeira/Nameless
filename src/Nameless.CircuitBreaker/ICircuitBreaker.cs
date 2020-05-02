using System;
using System.Threading.Tasks;

namespace Nameless.CircuitBreaker {
    public interface ICircuitBreaker {
        #region Events

        /// <summary>
        /// Add triggers when circuit breaker state changes.
        /// </summary>
        event EventHandler<StateChangeEventArgs> StateChanged;

        /// <summary>
        /// Add triggers when circuit breaker service level changes.
        /// </summary>
		event EventHandler<ServiceLevelEventArgs> ServiceLevelChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        long Threshold { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
		long Timeout { get; set; }

        /// <summary>
        /// Gets the current service level.
        /// </summary>
		double ServiceLevel { get; }

        /// <summary>
        /// Gets the circuit breaker current state.
        /// </summary>
		CircuitBreakerState State { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an ignore filter for exception.
        /// </summary>
        /// <param name="filter">The filter of the exception.</param>
        void IgnoreOn (IExceptionFilter filter);

        /// <summary>
        /// Executes the operation.
        /// </summary>
        /// <typeparam name="TResult">The operation result type.</typeparam>
        /// <param name="operation">The operation.</param>
        /// <param name="arguments">The operation arguments, if any.</param>
        /// <returns>The result of the operation.</returns>
        TResult Execute<TResult> (Delegate operation, params object[] arguments);

        /// <summary>
        /// Trips the circuit breaker.
        /// </summary>
		void Trip ();

        /// <summary>
        /// Reset the circuit breaker.
        /// </summary>
		void Reset ();

        #endregion
    }
}
