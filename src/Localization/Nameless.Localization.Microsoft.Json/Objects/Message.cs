using System.Diagnostics;

namespace Nameless.Localization.Microsoft.Json.Objects {
    [DebuggerDisplay("{ID}: {Text}")]
    public sealed record Message(string ID, string Text) {
        #region Public Static Read-Only Properties

        public static Message Empty => new(string.Empty, string.Empty);

        #endregion

        #region Public Properties

        public string ID { get; } = ID;
        public string Text { get; } = Text;

        #endregion
    }
}
