using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nameless.Localization.Json {
    public class JsonStringLocalizer : IStringLocalizer {

        #region Private Read-Only Fields

        private readonly IDictionary<string, LocalizableString> _cache = new Dictionary<string, LocalizableString> ();

        #endregion

        #region Public Properties

        public string ResourceLocation { get; }
        public string CultureName { get; }

        #endregion

        #region Public Constructors

        public JsonStringLocalizer (string resourceLocation, string cultureName) {
            Prevent.ParameterNullOrWhiteSpace (resourceLocation, nameof (resourceLocation));
            Prevent.ParameterNull (cultureName, nameof (cultureName));

            ResourceLocation = resourceLocation;
            CultureName = cultureName;

            Initialize ();
        }

        #endregion

        #region Internal Static Methods

        internal static string FormatResourceFilePath (string resourceLocation, string cultureName) {
            return !string.IsNullOrWhiteSpace (cultureName)
                ? $"{resourceLocation}.{cultureName}.json"
                : $"{resourceLocation}.json";
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            var resourceFilePath = FormatResourceFilePath (ResourceLocation, CultureName);
            if (!File.Exists (resourceFilePath)) { return; }
            JObject json;
            using (var streamReader = new StreamReader (resourceFilePath, Encoding.UTF8))
            using (var jsonTextReader = new JsonTextReader (streamReader)) {
                json = JObject.Load (jsonTextReader);
            }

            foreach (var property in json.Properties ()) {
                if (!_cache.ContainsKey (property.Name)) {
                    _cache.Add (property.Name, new LocalizableString {
                        Name = property.Name,
                        Value = property.Value.ToString (),
                        Culture = CultureName
                    });
                }
            }
        }

        private LocalizedString GetLocalizedString (string name, object[] arguments) {
            var found = _cache.TryGetValue (name, out LocalizableString localizableString);
            var translation = found ? localizableString.Value : name;
            return new LocalizedString (
                name: name,
                value: !arguments.IsNullOrEmpty ()
                    ? string.Format (translation, arguments)
                    : translation,
                resourceNotFound: !found,
                searchedLocation: ResourceLocation
            );
        }

        private IEnumerable<LocalizedString> GetAllStrings (string[] cultures) {
            foreach (var culture in cultures) {
                var localizer = (culture != CultureName) ? WithCulture (new CultureInfo (culture)) : this;
                foreach (var localizedString in localizer.GetAllStrings (includeParentCultures: false)) {
                    yield return localizedString;
                }
            }
        }

        private IEnumerable<LocalizedString> GetAllStrings () {
            foreach (var kvp in _cache) {
                yield return new LocalizedString (
                    name: kvp.Value.Name,
                    value: kvp.Value.Value,
                    resourceNotFound: true,
                    searchedLocation: ResourceLocation
                );
            }
        }

        #endregion

        #region IStringLocalizer Members

        public LocalizedString this[string name] => GetLocalizedString (name, arguments: null);

        public LocalizedString this[string name, params object[] arguments] => GetLocalizedString (name, arguments);

        public IEnumerable<LocalizedString> GetAllStrings (bool includeParentCultures) {
            return includeParentCultures
                ? GetAllStrings (LocalizationUtils.GetCultures (new CultureInfo (CultureName)))
                : GetAllStrings ();
        }

        public IStringLocalizer WithCulture (CultureInfo culture) {
            return new JsonStringLocalizer (ResourceLocation, culture.Name);
        }

        #endregion
    }
}