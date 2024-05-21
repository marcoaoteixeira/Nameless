using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.Localization.Microsoft.Json.Objects {
    [DebuggerDisplay("{Name}")]
    public sealed record Region(string Name, Message[] Messages) {
        #region Public Static Read-Only Properties

        public static Region Empty => new(string.Empty, []);

        #endregion

        #region Public Properties

        public string Name { get; } = Name;

        public Message[] Messages { get; } = Messages;

        #endregion

        #region Public Methods

        public bool TryGetMessage(string id, [NotNullWhen(true)] out Message? output) {
            var current = Messages.SingleOrDefault(item => id == item.ID);
            
            output = current;

            return current is not null;
        }

        #endregion
    }
}
