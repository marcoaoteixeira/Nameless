using System;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public static class Clock {
        #region Private Static Fields

        private static DateTime? frozen;

        #endregion

        #region Public Static Properties

        public static DateTime Now {
            get { return frozen ?? DateTime.UtcNow; }
        }

        public static DateTime Date {
            get { return Now.Date; }
        }

        #endregion

        #region  Public Static Methods

        public static IDisposable Freeze (DateTime dateTime) {
            frozen = dateTime;

            return new AnonymousDisposable (() => Unfreeze ());
        }

        #endregion

        #region Private Static Methods

        private static void Unfreeze () {
            frozen = null;
        }

        #endregion
    }
}