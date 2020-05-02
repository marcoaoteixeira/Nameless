using System.Collections.Generic;
using System.Linq;
using Nameless.Logging;

namespace Nameless.Notification {
    /// <summary>
    /// Default implementation of <see cref="INotifier"/>.
    /// </summary>
    public class Notifier : INotifier {

        #region Private Read-Only Fields

        private readonly IList<NotifyEntry> _entries = new List<NotifyEntry> ();

        #endregion

        #region Public Properties

        private ILogger _logger;

        /// <summary>
        /// Gets or sets the log system.
        /// </summary>
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region INotifier Members

        /// <inheritdoc/>
        public void Add (NotifyType type, string message) {
            Logger.Information ($"Notification {type} message: {message}");
            _entries.Add (new NotifyEntry { Type = type, Message = message });
        }

        /// <inheritdoc/>
        public IEnumerable<NotifyEntry> Flush () {
            var result = _entries.ToArray ();
            _entries.Clear ();
            return result;
        }

        #endregion
    }
}