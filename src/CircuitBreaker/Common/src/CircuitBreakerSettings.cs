namespace Nameless.CircuitBreaker.Common {
    public class CircuitBreakerSettings {
        #region Public Properties

        public long Threshold { get; set; } = 5;
        public long Timeout { get; set; } = 60_000;

        #endregion
    }
}