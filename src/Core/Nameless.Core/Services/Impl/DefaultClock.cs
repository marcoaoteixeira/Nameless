namespace Nameless.Services.Impl {

    /// <summary>
    /// Singleton Pattern implementation for <see cref="DefaultClock" />. (see: https://en.wikipedia.org/wiki/Singleton_pattern)
    /// </summary>
    [Singleton]
    public sealed class DefaultClock : IClock {

        #region Private Static Read-Only Fields

        private static readonly IClock _instance = new DefaultClock();

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="DefaultClock" />.
        /// </summary>
        public static IClock Instance => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static DefaultClock() { }

        #endregion

        #region Private Constructors

        private DefaultClock() { }

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
