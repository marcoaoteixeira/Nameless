namespace Nameless.Notification {

    /// <summary>
    /// Default implementation of <see cref="INotifier"/>.
    /// </summary>
    public sealed class Notifier : INotifier {

        #region Private Read-Only Fields

        private readonly List<NotifyEntry> _entries = new();

        #endregion

        #region INotifier Members

        /// <inheritdoc/>
        public void Add(NotifyType type, string message) {
            _entries.Add(new NotifyEntry { Type = type, Message = message });
        }

        /// <inheritdoc/>
        public IEnumerable<NotifyEntry> Flush() {
            var result = _entries.ToArray();
            _entries.Clear();
            return result;
        }

        #endregion
    }
}
