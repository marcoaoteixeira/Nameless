#if NET6_0_OR_GREATER
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif
using Nameless.Infrastructure;

namespace Nameless {
    public static class ArgCollectionExtension {
        #region Public Static Methods

        public static string ToJson(this ArgCollection? self)
#if NET6_0_OR_GREATER
            => self is not null ? JsonSerializer.Serialize(self) : string.Empty;
#else
            => self is not null ? JsonConvert.SerializeObject(self) : string.Empty;
#endif

        #endregion
    }
}
