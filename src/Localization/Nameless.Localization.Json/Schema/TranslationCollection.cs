namespace Nameless.Localization.Json.Schema {

    public sealed class TranslationCollection {

        #region Private Read-Only Fields

        private readonly Dictionary<string, TranslationEntry> _entries;

        #endregion

        #region Public Properties

        public string Key { get; }
        public TranslationEntry[] Values => _entries.Values.ToArray();

        #endregion

        #region Public Constructors

        public TranslationCollection(string key, IEnumerable<TranslationEntry>? values = default) {
            Prevent.NullOrWhiteSpaces(key, nameof(key));

            Key = key;
            _entries = (values ?? Array.Empty<TranslationEntry>()).ToDictionary(_ => _.Key, _ => _);
        }

        #endregion

        #region Public Methods

        public bool TryGetValue(string key, out TranslationEntry? output) => _entries.TryGetValue(key, out output);

        public bool Equals(TranslationCollection? other) => other != default && other.Key == Key;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj) => Equals(obj as TranslationCollection);

        public override int GetHashCode() => (Key ?? string.Empty).GetHashCode();

        #endregion
    }
}
