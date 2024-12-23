namespace Nameless;

/// <summary>
/// <see cref="DateTime"/> extension methods.
/// </summary>
public static class DateTimeExtension {
    /// <summary>
    /// Retrieves the difference, in years, between the <paramref name="self"/>
    /// and the <paramref name="date"/>. Note: Gregorian calendar.
    /// </summary>
    /// <param name="self">The current <see cref="DateTime"/>.</param>
    /// <param name="date">The other <see cref="DateTime"/>.</param>
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
    /// Converts a date into a Unix epoch date.
    /// </summary>
    /// <param name="self">The source <see cref="DateTime"/></param>
    /// <returns>A <see cref="long"/> representing the Unix epoch date.</returns>
    public static long ToUnixEpochDate(this DateTime self)
        => (long)Math.Round((self.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
}