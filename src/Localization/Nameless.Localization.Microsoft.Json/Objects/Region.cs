using System.Diagnostics.CodeAnalysis;

namespace Nameless.Localization.Microsoft.Json.Objects {
    public sealed class Region {
        #region Public Static Read-Only Properties

        public static Region Empty => new();

        #endregion

        #region Public Properties

        public string Name { get; init; } = string.Empty;
        public HashSet<Message> Messages { get; init; } = [];

        #endregion

        #region Public Methods

        public bool TryGetValue(string id, [NotNullWhen(true)] out Message? output)
            => Messages.TryGetValue(new() { ID = id }, out output);

        public bool Equals(Region? obj)
            => obj is not null &&
               obj.Name == Name;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Region);

        public override int GetHashCode()
            => (Name ?? string.Empty).GetHashCode();

        public override string ToString()
            => Name;

        #endregion
    }
}
