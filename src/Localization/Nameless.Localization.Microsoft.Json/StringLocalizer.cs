using System.Globalization;
using Microsoft.Extensions.Localization;
using Nameless.Localization.Microsoft.Json.Objects;

namespace Nameless.Localization.Microsoft.Json {
    public sealed class StringLocalizer : IStringLocalizer {
        #region Private Read-Only Fields

        private readonly CultureInfo _culture;
        private readonly string _resourceName;
        private readonly string _resourcePath;
        private readonly Region _region;
        private readonly Func<CultureInfo, string, string, IStringLocalizer> _factory;

        #endregion

        #region Private Properties

        public string Location => $"{_culture}::{_resourceName}::{_resourcePath}";

        #endregion

        #region Public Constructors

        public StringLocalizer(CultureInfo culture, string resourceName, string resourcePath, Region region, Func<CultureInfo, string, string, IStringLocalizer> factory) {
            _culture = Guard.Against.Null(culture, nameof(culture));
            _resourceName = Guard.Against.NullOrWhiteSpace(resourceName, nameof(resourceName));
            _resourcePath = Guard.Against.NullOrWhiteSpace(resourcePath, nameof(resourcePath));
            _region = Guard.Against.Null(region, nameof(region));
            _factory = Guard.Against.Null(factory, nameof(factory));
        }

        #endregion

        #region Private Methods

        private LocalizedString GetLocalizedString(string text, params object[] args) {
            var found = _region.TryGetValue(text, out var message);
            (var name, var value) = message is not null
                ? (message.ID, message.Text)
                : (text, text);

            return new(
                name: args.Length > 0 ? string.Format(name, args) : name,
                value: args.Length > 0 ? string.Format(value, args) : value,
                resourceNotFound: !found,
                searchedLocation: Location
            );
        }

        #endregion

        #region IStringLocalizer Members

        public LocalizedString this[string name]
            => GetLocalizedString(name);

        public LocalizedString this[string name, params object[] arguments]
            => GetLocalizedString(name, arguments);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) {
            foreach (var entry in _region) {
                yield return new(entry.ID, entry.Text, false, Location);
            }

            if (includeParentCultures) {
                foreach (var culture in _culture.GetParents().Skip(1)) {
                    var localizer = _factory(culture, _resourceName, _resourcePath);
                    foreach (var localeString in localizer.GetAllStrings(includeParentCultures: false)) {
                        yield return localeString;
                    }
                }
            }
        }

        #endregion
    }
}
