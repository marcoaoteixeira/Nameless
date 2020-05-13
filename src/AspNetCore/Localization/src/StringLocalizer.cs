using System.Collections.Generic;
using System.Globalization;

namespace Nameless.AspNetCore.Localization {
    public sealed class StringLocalizer : Microsoft.Extensions.Localization.IStringLocalizer {
        #region Private Read-Only Fields

        private readonly Nameless.Localization.IStringLocalizerFactory _factory;
        private readonly Nameless.Localization.IStringLocalizer _current;

        #endregion

        #region Public Properties

        public Microsoft.Extensions.Localization.LocalizedString this [string name, int count = -1, params object[] args] {
            get {
                var localizedString = _current.Get (name, count, args);
                return localizedString.Parse ();
            }
        }

        #endregion

        #region Public Constructors

        public StringLocalizer (Nameless.Localization.IStringLocalizerFactory factory, Nameless.Localization.IStringLocalizer current) {
            Prevent.ParameterNull (factory, nameof (factory));
            Prevent.ParameterNull (current, nameof (current));

            _factory = factory;
            _current = current;
        }

        #endregion

        #region Microsoft.Extensions.Localization.IStringLocalizer Members

        public Microsoft.Extensions.Localization.LocalizedString this [string name] => this [name, arguments : null];

        public Microsoft.Extensions.Localization.LocalizedString this [string name, params object[] arguments] => _current.Get (name, args : arguments).Parse ();

        public IEnumerable<Microsoft.Extensions.Localization.LocalizedString> GetAllStrings (bool includeParentCultures) {
            var currentCulture = new CultureInfo (_current.CultureName);
            var cultures = includeParentCultures ? currentCulture.GetTree () : new [] { currentCulture };

            foreach (var culture in cultures) {
                var localizer = _factory.Create (_current.BaseName, _current.Location, culture.Name);
                foreach (var localizedString in localizer.List ()) {
                    yield return localizedString.Parse ();
                }
            }
        }

        public Microsoft.Extensions.Localization.IStringLocalizer WithCulture (CultureInfo culture) {
            Prevent.ParameterNull (culture, nameof (culture));

            var localizer = _factory.Create (_current.BaseName, _current.Location, culture.Name);
            return new StringLocalizer (_factory, localizer);
        }

        #endregion
    }

    internal static class LocalizedStringExtension {
        #region Internal Static Methods

        internal static Microsoft.Extensions.Localization.LocalizedString Parse (this Nameless.Localization.LocalizedString self) {
            return new Microsoft.Extensions.Localization.LocalizedString (self.Text, self.ToString ());
        }

        #endregion
    }
}