namespace Nameless;

/// <summary>
///     <see cref="DateTime" /> extension methods.
/// </summary>
public static class DateTimeExtensions {
    /// <summary>
    ///     Retrieves the difference, in years, between the <paramref name="self" />
    ///     and the <paramref name="date" />. Note: Gregorian calendar.
    /// </summary>
    /// <param name="self">The current <see cref="DateTime" />.</param>
    /// <param name="date">The other <see cref="DateTime" />.</param>
    /// <returns>An integer representation of the difference.</returns>
    public static int GetYears(this DateTime self, DateTime date) {
        var zero = new DateTime(1, 1, 1); // Gregorian calendar.
        var span = self > date ? self - date : date - self;

        // Because we start at year 1 for the Gregorian
        // calendar, we must subtract a year here.
        var result = (zero + span).Year - 1;

        return result;
    }

    /// <summary>
    ///     Retrieves the <see cref="DateTime"/> as Unix (Epoch) time
    ///     in milliseconds.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="DateTime" />.
    /// </param>
    /// <returns>
    ///     A <see cref="long" /> representing the Unix (Epoch) time
    ///     in milliseconds.
    /// </returns>
    /// <remarks>
    ///     If the <see cref="DateTime.Kind" /> is not
    ///     <see cref="DateTimeKind.Utc" />, the method converts it to
    ///     UTC before calculating the Unix time.
    /// </remarks>
    public static long ToUnixTimeMilliseconds(this DateTime self) {
        var dateTime = self.Kind != DateTimeKind.Utc
            ? self.ToUniversalTime()
            : self;

        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }
}