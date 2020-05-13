using System;

namespace Nameless.AspNetCore.Localization {
    public sealed class StringLocalizerFactory : Microsoft.Extensions.Localization.IStringLocalizerFactory {

        #region Private Read-Only Fields

        private readonly Nameless.Localization.IStringLocalizerFactory _factory;

        #endregion

        #region Public Constructors

        public StringLocalizerFactory (Nameless.Localization.IStringLocalizerFactory factory) {
            Prevent.ParameterNull (factory, nameof (factory));

            _factory = factory;
        }

        #endregion

        #region Microsoft.Extensions.Localization.IStringLocalizerFactory Members

        public Microsoft.Extensions.Localization.IStringLocalizer Create (Type resourceSource) => Create (resourceSource.Assembly.GetName ().Name, resourceSource.FullName);

        public Microsoft.Extensions.Localization.IStringLocalizer Create (string baseName, string location) {
            Prevent.ParameterNullOrWhiteSpace (baseName, nameof (baseName));
            Prevent.ParameterNullOrWhiteSpace (location, nameof (location));

            var localizer = _factory.Create (baseName, location);

            return new StringLocalizer (_factory, localizer);
        }

        #endregion
    }
}