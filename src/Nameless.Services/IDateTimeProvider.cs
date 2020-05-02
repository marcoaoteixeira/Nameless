using System;

namespace Nameless.Services {
    /// <summary>
    /// Provides the current UTC <see cref="IDateTimeProvider"/>.
    /// This service should be used whenever the current date and time are needed, instead of <seealso cref="DateTime"/> directly.
    /// It also makes implementations more testable, as time can be mocked.
    /// </summary>
    public interface IDateTimeProvider {

        #region Properties

         /// <summary>
        /// Gets the current <see cref="DateTime"/> of the system
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the current <see cref="DateTime"/> of the system, expressed in UTC
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the current <see cref="DateTimeOffset"/> of the system.
        /// </summary>
        DateTimeOffset OffsetNow { get; }

        /// <summary>
        /// Gets the current <see cref="DateTimeOffset"/> of the system, expressed in UTC
        /// </summary>
        DateTimeOffset OffsetUtcNow { get; }

        #endregion
    }
}