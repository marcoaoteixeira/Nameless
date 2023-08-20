namespace Nameless.Localization.Microsoft.Json.Objects {
    public sealed record Message {
        #region Public Properties

        public string ID { get; }
        public string Text { get; }

        #endregion

        #region Public Constructors

        public Message(string id, string text) {
            ID = Guard.Against.Null(id, nameof(id));
            Text = Guard.Against.Null(text, nameof(text));
        }

        #endregion
    }
}
