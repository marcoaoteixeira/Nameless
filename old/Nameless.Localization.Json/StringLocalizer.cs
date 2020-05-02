using System.Collections.Generic;
using Nameless.FileProvider;
using Newtonsoft.Json;

namespace Nameless.Localization.Json {
    public sealed class StringLocalizer : IStringLocalizer {
        #region Private Read-Only Fields

        private readonly IFileProvider _fileProvider;
        private readonly string _path;

        #endregion

        #region Private Fields

        private IDictionary<string, string[]> Translations { get; set; }

        #endregion

        #region Public Constructors

        public StringLocalizer (IFileProvider fileProvider, string baseName, string location, string culture, PluralizationRuleDelegate plurarRule, string path) {
            Prevent.ParameterNull (fileProvider, nameof (fileProvider));
            Prevent.ParameterNullOrWhiteSpace (baseName, nameof (baseName));
            Prevent.ParameterNullOrWhiteSpace (location, nameof (location));
            Prevent.ParameterNullOrWhiteSpace (culture, nameof (culture));
            Prevent.ParameterNull (plurarRule, nameof (plurarRule));
            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));

            _fileProvider = fileProvider;

            BaseName = baseName;
            Location = location;
            Culture = culture;
            PluralizationRule = plurarRule;

            _path = path;

            Initialize ();
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            var json = _fileProvider.GetFile (_path).GetStream ().ToText ();
            Translations = JsonConvert.DeserializeObject<Dictionary<string, string[]>> (json);
        }

        #endregion

        #region IStringLocalizer

        public string BaseName { get; }
        public string Location { get; }
        public string Culture { get; }

        public PluralizationRuleDelegate PluralizationRule { get; }

        public LocalizableString Get (string name, int count = -1, params object[] args) {
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            if (!Translations.TryGetValue (name, out string[] translations)) {
                return new LocalizableString (name, args : args);
            }

            var pluralForm = PluralizationRule (count);
            if (pluralForm >= translations.Length) {
                throw new PluralFormNotFoundException ($"Plural form '{pluralForm}' doesn't exist for the key '{name}' in the '{Culture}' culture.");
            }

            return new LocalizableString (name, translations[pluralForm], args);
        }

        public IEnumerable<LocalizableString> List () {
            foreach (var kvp in Translations) {
                foreach (var translation in kvp.Value) {
                    yield return new LocalizableString (kvp.Key, translation);
                }
            }
        }

        #endregion
    }
}