namespace Nameless.Localization.Json.Schema {

    public sealed class TranslationEntry {

        #region Public Properties

        public string Key { get; }
        public string[] Values { get; }

        #endregion

        #region Public Constructors

        public TranslationEntry(string key, string[]? values = default) {
            Prevent.NullOrWhiteSpaces(key, nameof(key));

            Key = key;
            Values = values ?? Array.Empty<string>();
        }

        #endregion

        #region Public Methods

        public bool Equals(TranslationEntry? other) => other != default && other.Key == Key;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj) => Equals(obj as TranslationEntry);

        public override int GetHashCode() => (Key ?? string.Empty).GetHashCode();

        #endregion
    }
}
