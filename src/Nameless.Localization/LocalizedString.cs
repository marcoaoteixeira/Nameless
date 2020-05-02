namespace Nameless.Localization {
    public sealed class LocalizedString {
        #region Public Properties

        public string Text { get; }
        public string Translation { get; }
        public object[] Args { get; }

        #endregion

        #region Public Constructors

        public LocalizedString (string text, string translation = null, params object[] args) {
            Prevent.ParameterNullOrWhiteSpace (text, nameof (text));

            Text = text;
            Translation = translation ?? text;
            Args = args;
        }

        #endregion

        #region Public Methods

        public bool Equals (LocalizedString obj) => obj != null & obj.Text == Text && obj.Translation == Translation;

        #endregion

        #region Public Explicit Operator

        public static implicit operator string (LocalizedString localizationString) => localizationString?.ToString ();

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as LocalizedString);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += Text.GetHashCode () * 7;
                hash += Translation.GetHashCode () * 7;
            }
            return hash;
        }

        public override string ToString () => !Args.IsNullOrEmpty () ? string.Format (Translation, Args) : Translation;

        #endregion
    }
}