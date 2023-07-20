namespace Nameless.Localization.Json.Objects.Translation {
    public sealed record Leaf {
        #region Public Properties

        public string ID { get; }
        public string Text { get; }

        #endregion

        #region Public Constructors

        public Leaf(string id, string text) {
            ID = Prevent.Against.Null(id, nameof(id));
            Text = Prevent.Against.Null(text, nameof(text));
        }

        #endregion
    }
}
