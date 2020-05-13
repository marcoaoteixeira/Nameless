using System;

namespace Nameless.CircuitBreaker {
    /// <summary>
    /// Service level event arguments.
    /// </summary>
	public sealed class ServiceLevelEventArgs : EventArgs {

        #region Public Properties

        /// <summary>
        /// Gets the current service level.
        /// </summary>
        public double ServiceLevel { get; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceLevelEventArgs"/>;
        /// </summary>
        /// <param name="serviceLevel">The current service level.</param>
        public ServiceLevelEventArgs (double serviceLevel) {
            ServiceLevel = serviceLevel;
        }

        #endregion
    }
}
