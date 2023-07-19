using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nameless.Helpers;

namespace Nameless.Localization.Json.Schema {
    /// <summary>
    /// This class represents a translation, in other words, a file for a culture.
    /// </summary>
    public sealed class Translation {
        #region Private Read-Only Fields

        private readonly Dictionary<string, EntryCollection> _dictionary;

        #endregion

        #region Public Properties

        public CultureInfo Culture { get; }
        public EntryCollection[] EntryCollections => _dictionary.Values.ToArray();

        #endregion

        #region Public Constructors

        public Translation(CultureInfo culture, params EntryCollection[] entryCollections) {
            Culture = Prevent.Against.Null(culture, nameof(culture));

            _dictionary = (entryCollections ?? Array.Empty<EntryCollection>()).ToDictionary(_ => _.Source, _ => _);
        }

        #endregion

        #region Public Methods

        public bool TryGetValue(string source, [NotNullWhen(true)] out EntryCollection? output)
            => _dictionary.TryGetValue(source, out output);

        public bool Equals(Translation? other)
            => other != null
            && other.Culture.Name == Culture.Name;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Translation);

        public override int GetHashCode()
            => SimpleHash.Compute(Culture.Name);

        #endregion
    }
}
