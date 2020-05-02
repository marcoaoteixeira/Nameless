using System;
using System.Linq;

namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtension {

        #region Public Static Methods

        /// <summary>
        /// Gets the difference, in years, between the <paramref name="self"/> <see cref="DateTime"/> and <see cref="DateTime.Today"/>.
        /// </summary>
        /// <param name="self">The self <see cref="DateTime"/>.</param>
        /// <returns>An integer representation of the difference.</returns>
        public static int GetYearsToToday (this DateTime self) {
            return GetYears (self, DateTime.Today);
        }

        /// <summary>
        /// Gets the difference, in years, between the <paramref name="self"/> <see cref="DateTime"/> and the <paramref name="future"/> <see cref="DateTime"/>.
        /// </summary>
        /// <param name="self">The self <see cref="DateTime"/>.</param>
        /// <param name="future">The future <see cref="DateTime"/>.</param>
        /// <returns>An integer representation of the difference.</returns>
        public static int GetYears (this DateTime self, DateTime future) {
            var years = future.Year - self.Year;

            if (future.Month < self.Month) {--years; }
            if (future.Month == self.Month && future.Day < self.Day) {--years; }

            return years;
        }

        public static int GetFirstWorkingDay (int year, int month, int ordinal, params int[] holidays) {
            var day = 1;
            var workingDay = ordinal;
            while (workingDay != 0) {
                var date = new DateTime (year, month, day);
                bool weekend;
                switch (date.DayOfWeek) {
                    case DayOfWeek.Sunday:
                    case DayOfWeek.Saturday:
                        weekend = true;
                        break;
                    default:
                        weekend = false;
                        break;
                }
                if (!weekend) {
                    if (!holidays.Contains (day)) {
                        workingDay--;
                        day++;
                    }
                } else { day++; }
            }
            return day - 1;
        }

        public static long ToUnixEpochDate (this DateTime self) {
            return (long) Math.Round ((self.ToUniversalTime () - new DateTimeOffset (1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }

        #endregion
    }
}