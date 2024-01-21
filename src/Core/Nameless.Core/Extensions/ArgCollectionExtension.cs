using System.Text.Json;
using Nameless.Infrastructure;

namespace Nameless {
    public static class ArgCollectionExtension {
        #region Public Static Methods

        public static string ToJson(this ArgCollection? self)
            => self is not null
                ? JsonSerializer.Serialize(self.ToArray())
                : string.Empty;

        #endregion
    }
}
