namespace Nameless;

/// <summary>
///     <see cref="DateOnly"/> extension methods.
/// </summary>
public static class DateOnlyExtensions {
    /// <param name="self">
    ///     The current <see cref="DateOnly" />.
    /// </param>
    extension(DateOnly self) {
        /// <summary>
        ///     Retrieves the <see cref="DateOnly"/> as Unix (Epoch) time
        ///     in milliseconds.
        /// </summary>
        /// <returns>
        ///     A <see cref="long" /> representing the Unix (Epoch) time
        ///     in milliseconds.
        /// </returns>
        /// <remarks>
        ///     The method converts the <see cref="DateOnly"/> to a
        ///     <see cref="DateTime"/> object at midnight (00:00:00) UTC
        ///     before calculating the Unix time.
        /// </remarks>
        public long ToUnixTimeMilliseconds() {
            var dateTime = self.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }
    }
}