using System.Text.Json;

namespace Nameless.ProducerConsumer {
    public static class ArgCollectionExtension {
        #region Public Static Methods

        public static string ToJson(this ArgCollection? self)
            => self != null ? JsonSerializer.Serialize(self) : string.Empty;

        #endregion
    }
}
