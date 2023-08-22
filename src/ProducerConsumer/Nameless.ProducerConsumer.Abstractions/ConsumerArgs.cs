using Nameless.Infrastructure;

namespace Nameless.ProducerConsumer {
    public sealed class ConsumerArgs : ArgCollection {
        #region Public Static Read-Only Properties

        public static ConsumerArgs Empty => new();

        #endregion
    }
}
