namespace Nameless.Localization.Json.Schemas {
    public sealed class Message {
        #region Public Properties

        public string Text { get; }
        public string[] Translations { get; }

        #endregion

        #region Public Constructors

        public Message (string text, string[] translations) {
            Prevent.ParameterNullOrWhiteSpace (text, nameof (text));
            Prevent.ParameterNull (translations, nameof (translations));

            Text = text;
            Translations = translations;
        }

        #endregion

        #region Public Methods

        public LocalizedString GetTranslation (string text, int count = -1, PluralizationRuleDelegate pluralizationRule = null, params object[] args) {
            if (Translations.IsNullOrEmpty ()) {
                return new LocalizedString (text, text, args);
            }

            var pluralForm = (pluralizationRule ?? PluralizationRuleProvider.DefaultRule) (count);
            if (pluralForm >= Translations.Length) {
                throw new PluralFormNotFoundException ($"Couldn't locate plural form '{pluralForm}' message '{text}'.");
            }

            return new LocalizedString (text, Translations[pluralForm], args);
        }

        public bool Equals (Message obj) => obj != null && obj.Text == Text;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as Message);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (Text ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        public override string ToString () => Text;

        #endregion
    }
}