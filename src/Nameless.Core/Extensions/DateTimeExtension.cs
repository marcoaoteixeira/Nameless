﻿namespace Nameless;

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
    /// Retrieves the first working day of the given month and year.
    /// </summary>
    /// <param name="year">The year</param>
    /// <param name="month">The month</param>
    /// <param name="ordinal">The ordinal number for the working day, example 5 meaning 5th.</param>
    /// <param name="holidays">A list of holidays, for the specified month</param>
    /// <returns>The number of the first working day of the given month and year.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="holidays"/> is <c>null</c>.
    /// </exception>
    public static int GetFirstWorkingDay(int year, int month, int ordinal, params int[] holidays) {
        Prevent.Argument.Null(holidays);

        var day = 1;
        var workingDay = ordinal;
        while (workingDay != 0) {
            var date = new DateTime(year, month, day);
            var weekend = date.DayOfWeek switch {
                DayOfWeek.Saturday => true,
                DayOfWeek.Sunday => true,
                _ => false,
            };
            if (!weekend) {
                if (!holidays.Contains(day)) {
                    workingDay--;
                    day++;
                }
            } else { day++; }
        }

        return day - 1;
    }

    /// <summary>
    /// Converts a date into a Unix epoch date.
    /// </summary>
    /// <param name="self">The source <see cref="DateTime"/></param>
    /// <returns>A <see cref="long"/> representing the Unix epoch date.</returns>
    public static long ToUnixEpochDate(this DateTime self)
        => (long)Math.Round((self.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
}