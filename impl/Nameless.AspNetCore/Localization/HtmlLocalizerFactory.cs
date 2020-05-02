using System;

namespace Nameless.AspNetCore.Localization {
    public sealed class HtmlLocalizerFactory : Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizerFactory {
        #region Private Read-Only Fields

        private readonly Microsoft.Extensions.Localization.IStringLocalizerFactory _factory;

        #endregion

        #region Public Constructors

        public HtmlLocalizerFactory (Microsoft.Extensions.Localization.IStringLocalizerFactory factory) {
            Prevent.ParameterNull (factory, nameof (factory));

            _factory = factory;
        }

        #endregion

        #region Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizerFactory Members

        public Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer Create (Type resourceSource) => Create (resourceSource.Assembly.GetName ().Name, resourceSource.FullName);

        public Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer Create (string baseName, string location) {
            var localizer = _factory.Create (baseName, location);

            return new HtmlLocalizer (localizer);
        }

        #endregion
    }
}