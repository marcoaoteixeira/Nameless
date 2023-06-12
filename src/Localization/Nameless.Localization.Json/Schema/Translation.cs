using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nameless.Localization.Json.Schema {

    /// <summary>
    /// This class represents a translation, in other words, a file for a culture.
    /// </summary>
    public sealed class Translation {

        #region Private Read-Only Fields

        private readonly Dictionary<string, TranslationCollection> _entries;

        #endregion

        #region Public Properties

        public CultureInfo Culture { get; }
        public TranslationCollection[] Values => _entries.Values.ToArray();

        #endregion

        #region Public Constructors

        public Translation(CultureInfo culture, IEnumerable<TranslationCollection>? values = default) {
            Prevent.Null(culture, nameof(culture));

            Culture = culture;
            _entries = (values ?? Array.Empty<TranslationCollection>()).ToDictionary(_ => _.Key, _ => _);
        }

        #endregion

        #region Public Methods

        public bool TryGetValue(string key, [NotNullWhen(true)] out TranslationCollection? output) => _entries.TryGetValue(key, out output);

        public bool Equals(Translation? other) => other != default && other.Culture?.Name == Culture?.Name;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj) => Equals(obj as Translation);

        public override int GetHashCode() => (Culture?.Name ?? string.Empty).GetHashCode();

        #endregion
    }
}
