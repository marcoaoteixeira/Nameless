namespace Nameless.Localization.Json.Schema {
    public sealed record Entry {
        #region Public Properties

        public string Key { get; }
        public string Value { get; }

        #endregion

        #region Public Constructors

        public Entry(string key, string value) {
            Key = Prevent.Against.Null(key, nameof(key));
            Value = Prevent.Against.Null(value, nameof(value));
        }

        #endregion
    }
}
