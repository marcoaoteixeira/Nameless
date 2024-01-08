using System.Diagnostics.CodeAnalysis;

namespace Nameless.Localization.Microsoft.Json.Objects {
    /// <summary>
    /// This class represents a translation, in other words, a file for a culture.
    /// </summary>
    public sealed class Translation {
        #region Public Static Read-Only Properties

        public static Translation Empty => new();

        #endregion

        #region Public Properties

        public string Culture { get; set; } = string.Empty;
        public HashSet<Region> Regions { get; set; } = [];

        #endregion

        #region Public Methods

        public bool TryGetValue(string name, [NotNullWhen(true)] out Region? output)
            => Regions.TryGetValue(new() { Name = name }, out output);

        public bool Equals(Translation? obj)
            => obj is not null &&
               obj.Culture == Culture;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Translation);

        public override int GetHashCode()
            => (Culture ?? string.Empty).GetHashCode();

        public override string ToString()
            => Culture;

        #endregion
    }
}
