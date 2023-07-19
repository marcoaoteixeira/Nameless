using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Localization.Json.Schema;

namespace Nameless.Localization.Json {

    public sealed class StringLocalizer : IStringLocalizer {

        #region Private Read-Only Fields

        private readonly CultureInfo _culture;
        private readonly string _resourceName;
        private readonly string _resourcePath;
        private readonly PluralizationRuleDelegate _pluralizationRule;
        private readonly EntryCollection _translationCollection;
        private readonly Func<CultureInfo, string, string, IStringLocalizer> _factory;

        #endregion

        #region Private Fields

        private ILogger _logger = default!;

        #endregion

        #region Public Properties

        public ILogger Logger {
            get { return _logger ?? NullLogger.Instance; }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Public Constructors

        public StringLocalizer(CultureInfo culture, string resourceName, string resourcePath, PluralizationRuleDelegate pluralizationRule, EntryCollection translationCollection, Func<CultureInfo, string, string, IStringLocalizer> factory) {
            Prevent.Against.Null(culture, nameof(culture));
            Prevent.Against.NullOrWhiteSpace(resourceName, nameof(resourceName));
            Prevent.Against.NullOrWhiteSpace(resourcePath, nameof(resourcePath));
            Prevent.Against.Null(pluralizationRule, nameof(pluralizationRule));
            Prevent.Against.Null(translationCollection, nameof(translationCollection));
            Prevent.Against.Null(factory, nameof(factory));


            _culture = culture;
            _resourceName = resourceName;
            _resourcePath = resourcePath;
            _pluralizationRule = pluralizationRule;
            _translationCollection = translationCollection;
            _factory = factory;
        }

        #endregion

        #region IStringLocalizer Members

        public LocaleString this[string text, int count = -1, params object[] args] {
            get {
                if (!_translationCollection.TryGetValue(text, out var translation)) {
                    return new LocaleString(_culture, text, null, args);
                }

                var pluralForm = _pluralizationRule(count);
                if (pluralForm >= translation!.Values.Length) {
                    Logger.LogInformation($"Couldn't locate plural form '{pluralForm}' for message '{text}'.");
                    pluralForm = 0;
                }

                return new LocaleString(_culture, translation.Key, translation.Values[pluralForm], args);
            }
        }

        public IEnumerable<LocaleString> List(bool includeParentCultures = false) {
            foreach (var translation in _translationCollection.Entries) {
                foreach (var value in translation.Values) {
                    yield return new LocaleString(_culture, value, value);
                }
            }

            if (includeParentCultures) {
                foreach (var culture in _culture.GetParents().Skip(1)) {
                    var localizer = _factory(culture, _resourceName, _resourcePath);
                    foreach (var translation in localizer.List(includeParentCultures: false)) {
                        yield return translation;
                    }
                }
            }
        }

        #endregion
    }
}
