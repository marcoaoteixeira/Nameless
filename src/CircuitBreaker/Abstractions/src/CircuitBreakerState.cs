namespace Nameless.CircuitBreaker {
    /// <summary>
    /// Circuit breaker states
    /// </summary>
	public enum CircuitBreakerState {

        /// <summary>
        /// Closed
        /// </summary>
		Closed,

        /// <summary>
        /// Half open
        /// </summary>
		HalfOpen,

        /// <summary>
        /// Open
        /// </summary>
		Open
    }
}
