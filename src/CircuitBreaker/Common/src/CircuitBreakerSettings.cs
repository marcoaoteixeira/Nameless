namespace Nameless.CircuitBreaker {
    public class CircuitBreakerSettings {
        #region Public Properties

        /// <summary>
        /// Gets or sets the threshold. Default value is 5.
        /// </summary>
        public long Threshold { get; set; } = 5;
        public long Timeout { get; set; } = 60_000;

        #endregion
    }
}