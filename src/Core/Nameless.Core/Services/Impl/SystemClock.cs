namespace Nameless.Services.Impl {
    /// <summary>
    /// Singleton Pattern implementation for <see cref="SystemClock" />. (see: https://en.wikipedia.org/wiki/Singleton_pattern)
    /// </summary>
    [Singleton]
    public sealed class SystemClock : IClock {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="SystemClock" />.
        /// </summary>
        public static IClock Instance { get; } = new SystemClock();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static SystemClock() { }

        #endregion

        #region Private Constructors

        private SystemClock() { }

        #endregion

        #region IClockService Members

        public DateTime GetUtcNow()
            => DateTime.UtcNow;
        public DateTimeOffset GetUtcNowOffset()
            => new(new DateTime(
                ticks: DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond,
                kind: DateTimeKind.Utc
            ));

        #endregion
    }
}
