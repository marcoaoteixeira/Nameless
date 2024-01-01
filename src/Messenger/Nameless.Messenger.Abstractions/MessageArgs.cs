using Nameless.Infrastructure;

namespace Nameless.Messenger {
    public sealed class MessageArgs : ArgCollection {
        #region Public Static Read-Only Properties

        public static MessageArgs Default => new();

        #endregion
    }
}
