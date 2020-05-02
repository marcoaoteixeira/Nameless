using System.Collections.Generic;
using Nameless.Localization.Json.Schemas;

namespace Nameless.Localization.Json {
    public sealed class StringLocalizer : IStringLocalizer {
        #region Private Properties

        private MessageCollection MessageCollection { get; }

        #endregion

        #region Public Constructors

        public StringLocalizer (string baseName, string location, string cultureName, MessageCollection messageCollection, PluralizationRuleDelegate pluralizationRule) {
            Prevent.ParameterNullOrWhiteSpace (baseName, nameof (baseName));
            Prevent.ParameterNullOrWhiteSpace (location, nameof (location));
            Prevent.ParameterNullOrWhiteSpace (cultureName, nameof (cultureName));
            Prevent.ParameterNull (messageCollection, nameof (messageCollection));
            Prevent.ParameterNull (pluralizationRule, nameof (pluralizationRule));

            BaseName = baseName;
            Location = location;
            CultureName = cultureName;
            MessageCollection = messageCollection;
            PluralizationRule = pluralizationRule;
        }

        #endregion

        #region IStringLocalizer

        public string BaseName { get; }
        public string Location { get; }
        public string CultureName { get; }
        public PluralizationRuleDelegate PluralizationRule { get; }

        public LocalizedString Get (string text, int count = -1, params object[] args) {
            Prevent.ParameterNullOrWhiteSpace (text, nameof (text));

            return MessageCollection.GetTranslation (text, count, PluralizationRule, args);
        }

        public IEnumerable<LocalizedString> List () {
            foreach (var message in MessageCollection.Messages) {
                foreach (var translation in message.Translations) {
                    yield return new LocalizedString (message.Text, translation);
                }
            }
        }

        #endregion
    }
}