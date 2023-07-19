using System.Globalization;
using Nameless.Localization.Json.Schema;

namespace Nameless.Localization.Json {
    public sealed class StringLocalizer : IStringLocalizer {
        #region Private Read-Only Fields

        private readonly CultureInfo _culture;
        private readonly string _resourceName;
        private readonly string _resourcePath;
        private readonly Container _container;
        private readonly Func<CultureInfo, string, string, IStringLocalizer> _factory;

        #endregion

        #region Public Constructors

        public StringLocalizer(CultureInfo culture, string resourceName, string resourcePath, Container container, Func<CultureInfo, string, string, IStringLocalizer> factory) {
            _culture = Prevent.Against.Null(culture, nameof(culture));
            _resourceName = Prevent.Against.NullOrWhiteSpace(resourceName, nameof(resourceName));
            _resourcePath = Prevent.Against.NullOrWhiteSpace(resourcePath, nameof(resourcePath));
            _container = Prevent.Against.Null(container, nameof(container));
            _factory = Prevent.Against.Null(factory, nameof(factory));
        }

        #endregion

        #region IStringLocalizer Members

        public LocaleString this[string text, params object[] args] {
            get {
                return _container.TryGetValue(text, out var entry)
                    ? new(_culture, entry.Key, entry.Value, args)
                    : new(_culture, text, null, args);
            }
        }

        public IEnumerable<LocaleString> List(bool includeParentCultures = false) {
            foreach (var entry in _container) {
                yield return new(_culture, entry.Key, entry.Value);
            }

            if (includeParentCultures) {
                foreach (var culture in _culture.GetParents().Skip(1)) {
                    var localizer = _factory(culture, _resourceName, _resourcePath);
                    foreach (var localeString in localizer.List(includeParentCultures: false)) {
                        yield return localeString;
                    }
                }
            }
        }

        #endregion
    }
}
