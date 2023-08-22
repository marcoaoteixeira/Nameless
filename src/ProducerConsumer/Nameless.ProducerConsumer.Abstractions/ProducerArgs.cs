using Nameless.Infrastructure;

namespace Nameless.ProducerConsumer {
    public sealed class ProducerArgs : ArgCollection {
        #region Public Static Read-Only Properties

        public static ProducerArgs Empty => new();

        #endregion
    }
}
