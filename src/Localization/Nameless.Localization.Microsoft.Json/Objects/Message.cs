namespace Nameless.Localization.Microsoft.Json.Objects {
    public sealed class Message {
        #region Public Properties

        public string ID { get; init; } = string.Empty;
        public string Text { get; init; } = string.Empty;

        #endregion

        #region Public Methods

        public bool Equals(Message? obj)
            => obj is not null &&
               obj.ID == ID;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Message);

        public override int GetHashCode()
            => (ID ?? string.Empty).GetHashCode();

        public override string ToString()
            => $"{ID} : {Text}";

        #endregion
    }
}
