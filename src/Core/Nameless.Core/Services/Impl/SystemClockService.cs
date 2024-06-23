namespace Nameless.Services.Impl {
    /// <summary>
    /// Singleton Pattern implementation for <see cref="SystemClockService" />. (see: https://en.wikipedia.org/wiki/Singleton_pattern)
    /// </summary>
    [Singleton]
    public sealed class SystemClockService : IClockService {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="SystemClockService" />.
        /// </summary>
        public static IClockService Instance { get; } = new SystemClockService();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static SystemClockService() { }

        #endregion

        #region Private Constructors

        private SystemClockService() { }

        #endregion

        #region IClockService Members

        public DateTime GetUtcNow()
            => DateTime.UtcNow;
        public DateTimeOffset GetUtcNowOffset()
            => new(new DateTime(ticks: DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond,
                                kind: DateTimeKind.Utc));

        #endregion
    }
}
