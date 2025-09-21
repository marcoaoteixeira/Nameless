namespace Nameless;

/// <summary>
///     <see cref="DateOnly"/> extension methods.
/// </summary>
public static class DateOnlyExtensions {
    /// <summary>
    ///     Retrieves the <see cref="DateOnly"/> as Unix (Epoch) time
    ///     in milliseconds.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="DateOnly" />.
    /// </param>
    /// <returns>
    ///     A <see cref="long" /> representing the Unix (Epoch) time
    ///     in milliseconds.
    /// </returns>
    /// <remarks>
    ///     The method converts the <see cref="DateOnly"/> to a
    ///     <see cref="DateTime"/> object at midnight (00:00:00) UTC
    ///     before calculating the Unix time.
    /// </remarks>
    public static long ToUnixTimeMilliseconds(this DateOnly self) {
        var dateTime = self.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }
}
