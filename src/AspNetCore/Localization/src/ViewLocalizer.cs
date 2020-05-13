using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Nameless.AspNetCore.Localization {
    public sealed class ViewLocalizer : Microsoft.AspNetCore.Mvc.Localization.IViewLocalizer, IViewContextAware {
        #region Private Read-Only Fields

        private readonly Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizerFactory _factory;
        private readonly IWebHostEnvironment _environment;

        #endregion

        #region Private Fields

        private Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer _localizer;

        #endregion

        #region Public Constructors

        public ViewLocalizer (Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizerFactory factory, IWebHostEnvironment environment) {
            Prevent.ParameterNull (factory, nameof (factory));
            Prevent.ParameterNull (environment, nameof (environment));

            _factory = factory;
            _environment = environment;
        }

        #endregion

        #region Public Properties

        public Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString this [string name, int count = -1, params object[] args] {
            get {
                var localizedString = _localizer is HtmlLocalizer localizer ? localizer[name, count, args] : _localizer[name, arguments : args];
                return localizedString;
            }
        }

        #endregion

        #region Private Methods

        private string ExtractLocation (string path) {
            var extension = Path.GetExtension (path);
            var startIndex = path[0] == '/' || path[0] == '\\' ? 1 : 0;
            var length = path.Length - startIndex - extension.Length;
            var capacity = length + _environment.ApplicationName.Length + 1;
            var builder = new StringBuilder (path, startIndex, length, capacity);

            builder.Replace ('/', '.').Replace ('\\', '.');

            // Prepend the application name
            builder.Insert (0, '.');
            builder.Insert (0, _environment.ApplicationName);

            return builder.ToString ();
        }

        #endregion

        #region Microsoft.AspNetCore.Mvc.Localization.IViewLocalizer Members

        public Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString this [string name] => _localizer[name];

        public Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString this [string name, params object[] arguments] => _localizer[name, arguments];

        public IEnumerable<Microsoft.Extensions.Localization.LocalizedString> GetAllStrings (bool includeParentCultures) => _localizer.GetAllStrings (includeParentCultures);

        public Microsoft.Extensions.Localization.LocalizedString GetString (string name) => _localizer.GetString (name);

        public Microsoft.Extensions.Localization.LocalizedString GetString (string name, params object[] arguments) => _localizer.GetString (name, arguments);

        [Obsolete]
        public Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer WithCulture (CultureInfo culture) => _localizer.WithCulture (culture);

        #endregion

        #region Microsoft.AspNetCore.Mvc.ViewFeatures.IViewContextAware Members

        void IViewContextAware.Contextualize (ViewContext viewContext) {
            Prevent.ParameterNull (viewContext, nameof (viewContext));

            var path = viewContext.ExecutingFilePath;
            if (string.IsNullOrWhiteSpace (path)) {
                path = viewContext.View.Path;
            }

            var location = ExtractLocation (path);
            _localizer = _factory.Create (_environment.ApplicationName, location);
        }

        #endregion
    }
}