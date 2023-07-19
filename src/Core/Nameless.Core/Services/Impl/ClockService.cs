namespace Nameless.Services.Impl {
    /// <summary>
    /// Singleton Pattern implementation for <see cref="ClockService" />. (see: https://en.wikipedia.org/wiki/Singleton_pattern)
    /// </summary>
    [Singleton]
    public sealed class ClockService : IClockService {
        #region Private Static Read-Only Fields

        private static readonly ClockService _instance = new();

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="ClockService" />.
        /// </summary>
        public static IClockService Instance => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static ClockService() { }

        #endregion

        #region Private Constructors

        private ClockService() { }

        #endregion

        #region IClock Members

        public DateTime UtcNow => DateTime.UtcNow;
        public DateTimeOffset OffsetUtcNow {
            get {
                return new(new DateTime(
                    ticks: DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond,
                    kind: DateTimeKind.Utc
                ));
            }
        }

        #endregion
    }
}
