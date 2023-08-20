using Nameless.Infrastructure;

namespace Nameless.Messenger {
    public sealed class MessengerArgs : ArgCollection {
        #region Public Static Read-Only Properties

        public static MessengerArgs Default => new();

        #endregion
    }
}
