using System;

namespace Nameless.Services.Common {
    public sealed class DateTimeProvider : IDateTimeProvider {
        #region IDateTimeProvider Members

        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTimeOffset OffsetNow => DateTimeOffset.Now;

        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;

        #endregion
    }
}